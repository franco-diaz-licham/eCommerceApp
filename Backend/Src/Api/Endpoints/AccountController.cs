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


        return NoContent();
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreateOrUpdateAddressAsync(Address address)
    {

        return Ok();
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<Address>> GetSavedAddressAsync()
    {

        return Ok();
    }
}
