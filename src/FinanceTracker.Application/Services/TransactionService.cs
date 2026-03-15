using FinanceTracker.Application.Dtos;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Mapping;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public sealed class TransactionService(
    ITransactionRepository transactionRepo,
    ICategoryRepository categoryRepo) : ITransactionService
{
    public async Task<List<TransactionReadDto>> GetAllAsync(CancellationToken ct)
    {
        var transactions = await transactionRepo.GetAllNewestFirstAsync(ct);
        return transactions.ToDto();
    }

    public async Task<TransactionReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var transaction = await transactionRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Transaction with id {id} not found.");

        return transaction.ToDto();
    }

    public async Task<TransactionReadDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct)
    {
        var category = await categoryRepo.GetByIdAsync(dto.CategoryId, ct)
            ?? throw new KeyNotFoundException($"Category with id {dto.CategoryId} not found.");

        var entity = dto.ToEntity();
        await transactionRepo.AddAsync(entity, ct);
        await transactionRepo.SaveChangesAsync(ct);

        return entity.ToDto();
    }

    public async Task<TransactionReadDto> UpdateAsync(int id, TransactionCreateDto dto, CancellationToken ct)
    {
        var transaction = await transactionRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Transaction with id {id} not found.");

        var category = await categoryRepo.GetByIdAsync(dto.CategoryId, ct)
            ?? throw new KeyNotFoundException($"Category with id {dto.CategoryId} not found.");

        transaction.ApplyUpdate(dto);
        transactionRepo.Update(transaction);
        await transactionRepo.SaveChangesAsync(ct);

        return transaction.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var transaction = await transactionRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Transaction with id {id} not found.");

        transactionRepo.Remove(transaction);
        await transactionRepo.SaveChangesAsync(ct);
    }
}
