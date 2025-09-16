namespace Backend.Src.Infrastructure.Repositories;

public sealed class UserRepository(SignInManager<UserEntity> signInManager) : IUserRepository
{
    public async Task<UserEntity?> FindByIdAsync(string userId) => await signInManager.UserManager.FindByIdAsync(userId);

    public async Task<IReadOnlyCollection<string>> GetRolesAsync(UserEntity user) => (await signInManager.UserManager.GetRolesAsync(user)).ToList();

    public async Task<UserEntity> RegisterUserAsync(string email, string password)
    {
        var user = new UserEntity(email, email);
        var exists = await signInManager.UserManager.FindByEmailAsync(email);

        if(exists != null) throw new Exception("User already exists.");

        var result = await signInManager.UserManager.CreateAsync(user, password);
        if (!result.Succeeded) throw new Exception("Problem adding user.");

        await signInManager.UserManager.AddToRoleAsync(user, "Member");
        return user;
    }

    public async Task<UserEntity?> ReadUserProfileAsync(UserDto dto)
    {
        var user = await signInManager.UserManager.Users.Where(x => x.UserName == dto.UserName).Include(x => x.Address).FirstOrDefaultAsync();
        return user;
    }

    public async Task<bool> SignOutUserAsync()
    {
        await signInManager.SignOutAsync();
        return true;
    }
}
