namespace Backend.Src.Application.Services;

public interface IAccountService
{
    Task<Result<AddressDto>> CreateAddressAsync(AddressCreateDto dto);
    Task<Result<AddressDto>> GetUserAddressAsync();
    Task<Result<UserDto>> GetUserInfoAsync();
    Task<Result<UserDto>> RegisterUser(UserRegisterDto dto);
    Task<Result<bool>> SignoutAsync();
    Task<Result<bool>> UpdateAddressAysnc(AddressUpdateDto dto);
}