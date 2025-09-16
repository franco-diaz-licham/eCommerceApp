namespace Backend.Src.Api.Helpers;

public sealed class HttpCurrentUser(IHttpContextAccessor http) : ICurrentUser
{
    public bool IsAuthenticated => http.HttpContext?.User?.Identity?.IsAuthenticated == true;
    public string? UserId => http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public IReadOnlyCollection<string> Roles => http.HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
}
