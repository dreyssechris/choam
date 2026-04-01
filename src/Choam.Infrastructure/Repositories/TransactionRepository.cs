using Choam.Domain.Entities;
using Choam.Domain.Interfaces;
using Choam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Choam.Infrastructure.Repositories;

public sealed class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllNewestFirstAsync(string userId, CancellationToken ct)
        => await context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToListAsync(ct);

    public async Task<Transaction?> GetByIdAsync(int id, string userId, CancellationToken ct)
        => await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

    public async Task<List<Transaction>> GetByCategoryIdAsync(int categoryId, string userId, CancellationToken ct)
        => await context.Transactions
            .Where(t => t.CategoryId == categoryId && t.UserId == userId)
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
