namespace Backend.Src.Api.Models.Request;

public class UserRegisterRequest
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required, MinLength(7)] public string Password { get; set; } = default!;
}
