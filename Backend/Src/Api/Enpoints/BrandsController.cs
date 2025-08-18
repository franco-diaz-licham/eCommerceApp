namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class BrandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IBrandService _brandService;
    private readonly IPaginationService _paginationService;

    public BrandsController(IMapper mapper, IBrandService BrandService, IPaginationService paginationService)
    {
        _brandService = BrandService;
        _paginationService = paginationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<BrandResponse>>> GetBrandsAsync([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = _brandService.GetAllAsync(query);
        var paged = await _paginationService.ApplyPaginationAsync(models, query.PageNumber, query.PageSize);
        var output = _mapper.Map<PagedList<BrandResponse>>(paged);
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
