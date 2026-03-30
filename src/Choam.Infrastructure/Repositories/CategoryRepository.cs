using Choam.Domain.Entities;
using Choam.Domain.Interfaces;
using Choam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Choam.Infrastructure.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync(CancellationToken ct)
        => await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Categories.FindAsync([id], ct);

    public async Task<Category?> GetByNameAsync(string name, CancellationToken ct)
        => await context.Categories
            .FirstOrDefaultAsync(c => c.Name == name, ct);

    public async Task AddAsync(Category category, CancellationToken ct)
        => await context.Categories.AddAsync(category, ct);

    public void Update(Category category)
        => context.Categories.Update(category);

    public void Remove(Category category)
        => context.Categories.Remove(category);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);
}
