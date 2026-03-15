using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllNewestFirstAsync(CancellationToken ct = default);
    Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Transaction>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    void Update(Transaction transaction);
    void Remove(Transaction transaction);
    Task SaveChangesAsync(CancellationToken ct = default);
}
