using Choam.Domain.Entities;

namespace Choam.Domain.Interfaces;

public interface IAppUserRepository
{
    Task<AppUser?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(AppUser user, CancellationToken ct = default);
    void Update(AppUser user);
    Task SaveChangesAsync(CancellationToken ct = default);
}
