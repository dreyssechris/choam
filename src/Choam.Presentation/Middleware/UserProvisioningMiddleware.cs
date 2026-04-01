using Choam.Domain.Entities;
using Choam.Domain.Interfaces;

namespace Choam.Presentation.Middleware;

public class UserProvisioningMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAppUserRepository userRepo)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (userId is not null)
            {
                var existing = await userRepo.GetByIdAsync(userId);
                if (existing is null)
                {
                    var user = new AppUser
                    {
                        Id = userId,
                        UserName = context.User.FindFirst("preferred_username")?.Value ?? "",
                        Email = context.User.FindFirst("email")?.Value ?? "",
                        FirstLogin = DateTime.UtcNow,
                    };
                    await userRepo.AddAsync(user);

                    // Create default "Uncategorized" category for new user
                    var categoryRepo = context.RequestServices.GetRequiredService<ICategoryRepository>();
                    await categoryRepo.AddAsync(new Category
                    {
                        Name = "Uncategorized",
                        UserId = userId,
                    });

                    await userRepo.SaveChangesAsync();
                }
            }
        }

        await next(context);
    }
}
