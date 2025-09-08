namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BrandsController(IMapper mapper, IBrandService BrandService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IBrandService _brandService = BrandService;

    [HttpGet]
    public async Task<ActionResult<PagedList<BrandResponse>>> GetBrandsAsync([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = await _brandService.GetAllAsync(query);
        var output = _mapper.Map<PagedList<BrandResponse>>(models);
        Response.AddPaginationHeader(output.Metadata);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponse>> GetBrandAsync(int id)
    {
        var result = await _brandService.GetAsync(id);
        return _mapper.Map<Result<BrandResponse>>(result).ToActionResult();
    }
}
