using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public sealed class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllNewestFirstAsync(CancellationToken ct)
        => await context.Transactions
            .AsNoTracking()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToListAsync(ct);

    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Transactions.FindAsync([id], ct);

    public async Task<List<Transaction>> GetByCategoryIdAsync(int categoryId, CancellationToken ct)
        => await context.Transactions
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync(ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct)
        => await context.Transactions.AddAsync(transaction, ct);

    public void Update(Transaction transaction)
        => context.Transactions.Update(transaction);

    public void Remove(Transaction transaction)
        => context.Transactions.Remove(transaction);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);
}
