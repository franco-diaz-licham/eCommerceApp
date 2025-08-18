namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BuggyController : ControllerBase   
{
    [HttpGet("not-found")]
    public IActionResult GetNotFound() => NotFound(new ApiResponse(404, "Not found..."));

    [HttpGet("bad-request")]
    public IActionResult GetBadRequest() => BadRequest(new ApiResponse(400, "This is not a good request"));

    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorised() => Unauthorized(new ApiResponse(401, "You are not authorised..."));

    [HttpGet("forbidden")]
    public IActionResult GetForbidden() => Forbid();

    [HttpGet("server-error")]
    public IActionResult GetServerError() => throw new Exception("This is a server error");

    [HttpPost("validation-error")]
    public IActionResult GetValidationError(Validation model)
    {
        return ValidationProblem();
    }

    public class Validation
    {
        [Required] public required string Required1 { get; set; }
        [Required] public required string Required2 { get; set; }
    }
}



