using Choam.Application.Dtos;
using Choam.Application.Interfaces;
using Choam.Application.Mapping;
using Choam.Domain.Interfaces;

namespace Choam.Application.Services;

public sealed class CategoryService(
    ICategoryRepository categoryRepo,
    ITransactionRepository transactionRepo,
    ICurrentUserService currentUser) : ICategoryService
{
    private const string UncategorizedName = "Uncategorized";

    public async Task<List<CategoryReadDto>> GetAllAsync(CancellationToken ct)
    {
        var categories = await categoryRepo.GetAllAsync(currentUser.UserId, ct);
        return categories.ToDto();
    }

    public async Task<CategoryReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(id, currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        return category.ToDto();
    }

    public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct)
    {
        var existing = await categoryRepo.GetByNameAsync(dto.Name, currentUser.UserId, ct);
        if (existing is not null)
            throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

        var entity = dto.ToEntity();
        entity.UserId = currentUser.UserId;
        await categoryRepo.AddAsync(entity, ct);
        await categoryRepo.SaveChangesAsync(ct);

        return entity.ToDto();
    }

    public async Task<CategoryReadDto> UpdateAsync(int id, CategoryCreateDto dto, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(id, currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        if (category.Name == UncategorizedName)
            throw new InvalidOperationException("The default 'Uncategorized' category cannot be renamed.");

        var duplicate = await categoryRepo.GetByNameAsync(dto.Name, currentUser.UserId, ct);
        if (duplicate is not null && duplicate.Id != id)
            throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

        category.Name = dto.Name;
        categoryRepo.Update(category);
        await categoryRepo.SaveChangesAsync(ct);

        return category.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(id, currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        if (category.Name == UncategorizedName)
            throw new InvalidOperationException("The default 'Uncategorized' category cannot be deleted.");

        // Reassign orphaned transactions to user's "Uncategorized"
        var uncategorized = await categoryRepo.GetByNameAsync(UncategorizedName, currentUser.UserId, ct)
            ?? throw new InvalidOperationException("User has no 'Uncategorized' category.");

        var orphans = await transactionRepo.GetByCategoryIdAsync(id, currentUser.UserId, ct);
        foreach (var tx in orphans)
        {
            tx.CategoryId = uncategorized.Id;
            transactionRepo.Update(tx);
        }

        categoryRepo.Remove(category);
        await categoryRepo.SaveChangesAsync(ct);
    }
}
