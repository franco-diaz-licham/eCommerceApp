namespace Backend.Src.Application.Services;

public class AccountService : IAccountService
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public AccountService(ICurrentUser currentUser, IUserRepository userRepo, IMapper mapper)
    {
        _currentUser = currentUser;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> GetUserInfoAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId)) return Result<UserDto>.Fail("User does not hasve an id.", ResultTypeEnum.Invalid);

        var user = await _userRepo.FindByIdAsync(_currentUser.UserId!);
        if (user is null) return Result<UserDto>.Fail("User not found.", ResultTypeEnum.NotFound);

        var dto = _mapper.Map<UserDto>(user);
        dto.Roles = (await _userRepo.GetRolesAsync(user)).ToList();
        return Result<UserDto>.Success(dto, ResultTypeEnum.Success);
    }

    public async Task<Result<UserDto>> RegisterUser(UserRegisterDto dto)
    {
        var user = await _userRepo.RegisterUserAsync(dto.Email, dto.Password);
        var output = _mapper.Map<UserDto>(user);
        output.Roles = (await _userRepo.GetRolesAsync(user)).ToList();
        return Result<UserDto>.Success(output, ResultTypeEnum.Success);
    }

    public async Task<Result<bool>> SignoutAsync()
    {
        await _userRepo.SignOutUserAsync();
        return Result<bool>.Success(ResultTypeEnum.Success);
    }

    public async Task UpdateAysnc()
    {

    }
}
