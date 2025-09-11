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
        var response = _mapper.Map<Result<List<BrandResponse>>>(models);
        var output = response.ToActionPaginatedResult(Response, query.PageNumber, query.PageSize, response.TotalCount);
        return output;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponse>> GetBrandAsync(int id)
    {
        var result = await _brandService.GetAsync(id);
        return _mapper.Map<Result<BrandResponse>>(result).ToActionResult();
    }
}
