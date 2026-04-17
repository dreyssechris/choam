using Choam.Application.Interfaces;
using Choam.Domain.Interfaces;
using Choam.Infrastructure.Data;
using Choam.Infrastructure.ExternalServices;
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

        services.AddHttpClient<IOllamaClient, OllamaClient>(client =>
        {
            var ollamaUrl = configuration.GetValue<string>("Ollama:Url") ?? "http://localhost:11434";
            client.BaseAddress = new Uri(ollamaUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return services;
    }
}
