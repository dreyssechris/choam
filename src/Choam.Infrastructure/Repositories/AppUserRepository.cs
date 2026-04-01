using Choam.Domain.Entities;
using Choam.Domain.Interfaces;
using Choam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Choam.Infrastructure.Repositories;

public sealed class AppUserRepository(AppDbContext context) : IAppUserRepository
{
    public async Task<AppUser?> GetByIdAsync(string id, CancellationToken ct)
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct)
        => await context.Users.AddAsync(user, ct);

    public void Update(AppUser user)
        => context.Users.Update(user);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);
}
