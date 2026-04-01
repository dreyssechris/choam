namespace Choam.Application.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsDirector { get; }
}
