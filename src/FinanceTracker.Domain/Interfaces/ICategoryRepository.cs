using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    void Remove(Category category);
    Task SaveChangesAsync(CancellationToken ct = default);
}
