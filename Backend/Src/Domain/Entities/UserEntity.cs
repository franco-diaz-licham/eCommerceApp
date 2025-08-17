namespace Backend.Src.Domain.Entities;

public class UserEntity : IdentityUser
{
    public int UserId { get; set; }
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
}
