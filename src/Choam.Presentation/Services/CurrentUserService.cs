using System.Security.Claims;
using Choam.Application.Interfaces;

namespace Choam.Presentation.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No authenticated user.");

    public string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException("User ID claim not found.");

    public string UserName => User.FindFirstValue("preferred_username")
        ?? User.FindFirstValue(ClaimTypes.Name)
        ?? "";

    public IReadOnlyList<string> Roles => User.FindAll("roles")
        .Select(c => c.Value)
        .ToList();

    public bool IsDirector => Roles.Contains("director");
}
