namespace Backend.Src.Domain.Entities;

public class UserEntity : IdentityUser
{
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
}
