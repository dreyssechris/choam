using Choam.Domain.Entities;

namespace Choam.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(string userId, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, string userId, CancellationToken ct = default);
    Task<Category?> GetByNameAsync(string name, string userId, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    void Remove(Category category);
    Task SaveChangesAsync(CancellationToken ct = default);
}
