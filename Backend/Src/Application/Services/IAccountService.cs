
namespace Backend.Src.Application.Services;

public interface IAccountService
{
    Task<Result<UserDto>> GetUserInfoAsync();
    Task<Result<UserDto>> RegisterUser(UserRegisterDto dto);
}