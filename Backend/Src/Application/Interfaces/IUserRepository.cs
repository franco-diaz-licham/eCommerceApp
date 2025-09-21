namespace Backend.Src.Application.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> FindByIdAsync(string userId);
    Task<IReadOnlyCollection<string>> GetRolesAsync(UserEntity user);
    Task<UserEntity?> ReadUserProfileByIdAsync(string id);
    Task<UserEntity> RegisterUserAsync(string email, string password);
    Task<bool> SignOutUserAsync();
    Task UpdateAsync(UserEntity user);
}
