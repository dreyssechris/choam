using Choam.Domain.Interfaces;
using Choam.Infrastructure.Data;
using Choam.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Choam.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();

        return services;
    }
}
