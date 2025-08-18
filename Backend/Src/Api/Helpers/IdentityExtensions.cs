namespace Backend.Src.Api.Helpers;

public static class IdentityExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.Identity?.Name ?? throw new UnauthorizedAccessException();
    }
}
