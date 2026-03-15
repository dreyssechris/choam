using FinanceTracker.Application.Dtos;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Mapping;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public sealed class CategoryService(
    ICategoryRepository categoryRepo,
    ITransactionRepository transactionRepo) : ICategoryService
{
    private const int UncategorizedId = 1;

    public async Task<List<CategoryReadDto>> GetAllAsync(CancellationToken ct)
    {
        var categories = await categoryRepo.GetAllAsync(ct);
        return categories.ToDto();
    }

    public async Task<CategoryReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        return category.ToDto();
    }

    public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct)
    {
        var existing = await categoryRepo.GetByNameAsync(dto.Name, ct);
        if (existing is not null)
            throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

        var entity = dto.ToEntity();
        await categoryRepo.AddAsync(entity, ct);
        await categoryRepo.SaveChangesAsync(ct);

        return entity.ToDto();
    }

    public async Task<CategoryReadDto> UpdateAsync(int id, CategoryCreateDto dto, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        if (id == UncategorizedId)
            throw new InvalidOperationException("The default 'Uncategorized' category cannot be renamed.");

        var duplicate = await categoryRepo.GetByNameAsync(dto.Name, ct);
        if (duplicate is not null && duplicate.Id != id)
            throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

        category.Name = dto.Name;
        categoryRepo.Update(category);
        await categoryRepo.SaveChangesAsync(ct);

        return category.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        if (id == UncategorizedId)
            throw new InvalidOperationException("The default 'Uncategorized' category cannot be deleted.");

        var category = await categoryRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        // Reassign orphaned transactions to "Uncategorized"
        var orphans = await transactionRepo.GetByCategoryIdAsync(id, ct);
        foreach (var tx in orphans)
        {
            tx.CategoryId = UncategorizedId;
            transactionRepo.Update(tx);
        }

        categoryRepo.Remove(category);
        await categoryRepo.SaveChangesAsync(ct);
    }
}
