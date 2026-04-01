using Choam.Domain.Entities;

namespace Choam.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllNewestFirstAsync(string userId, CancellationToken ct = default);
    Task<Transaction?> GetByIdAsync(int id, string userId, CancellationToken ct = default);
    Task<List<Transaction>> GetByCategoryIdAsync(int categoryId, string userId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    void Update(Transaction transaction);
    void Remove(Transaction transaction);
    Task SaveChangesAsync(CancellationToken ct = default);
}
