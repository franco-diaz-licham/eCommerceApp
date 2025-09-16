namespace Backend.Src.Api.Models.Response;

public class UserResponse
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int AddressId { get; set; }
    public AddressDto? Address { get; set; }
    public List<string>? Roles { get; set; }
}
