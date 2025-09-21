namespace Backend.Src.Application.DTOs;

public class UserDto
{
    public bool IsAuthenticated { get; set;  }
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int? AddressId { get; set; }
    public AddressDto? Address { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = [];
}
