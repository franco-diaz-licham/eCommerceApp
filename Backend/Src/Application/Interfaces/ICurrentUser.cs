namespace Backend.Src.Application.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    IReadOnlyCollection<string> Roles { get; }
}
