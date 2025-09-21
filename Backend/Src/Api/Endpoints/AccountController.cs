namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService, IMapper mapper) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("signup")]
    public async Task<ActionResult<UserResponse>> RegisterUserAsync(UserRegisterRequest request)
    {
        var registerDto = _mapper.Map<UserRegisterDto>(request);
        var userDto = await _accountService.RegisterUser(registerDto);
        var output = _mapper.Map<Result<UserResponse>>(userDto);
        return output.ToActionResult();
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult> GetUserInfoAsync()
    {
        var userDto = await _accountService.GetUserInfoAsync();
        var output = _mapper.Map<Result<UserResponse>>(userDto);
        return output.ToActionResult();
    }

    [HttpPost("signout")]
    public async Task<ActionResult> SignoutAsync()
    {
        var output = await _accountService.SignoutAsync();
        return output.ToActionResult();
    }


    [HttpPost("address")]
    public async Task<ActionResult> CreateAddressAsync(CreateAddressRequest request)
    {
        var dto = _mapper.Map<AddressCreateDto>(request);
        var userDto = await _accountService.CreateAddressAsync(dto);
        var output = _mapper.Map<Result<UserResponse>>(userDto);
        return output.ToActionResult();
    }

    [HttpPut("address")]
    public async Task<ActionResult<bool>> UpdateAddressAsync(UpdateAddressRequest request)
    {
        var dto = _mapper.Map<AddressUpdateDto>(request);
        var userDto = await _accountService.UpdateAddressAysnc(dto);
        var output = _mapper.Map<Result<bool>>(userDto);
        return output.ToActionResult();
    }
}
