
namespace Backend.Src.Application.Services;

public interface IAccountService
{
    Task<Result<UserDto>> CreateAddressAsync(AddressCreateDto dto);
    Task<Result<UserDto>> GetUserInfoAsync();
    Task<Result<UserDto>> RegisterUser(UserRegisterDto dto);
    Task<Result<bool>> SignoutAsync();
    Task<Result<bool>> UpdateAddressAysnc(AddressUpdateDto dto);
}