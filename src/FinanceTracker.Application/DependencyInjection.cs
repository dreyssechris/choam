using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}
