namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class ProductTypesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProductTypeService _productTypeService;
    private readonly IPaginationService _paginationService;

    public ProductTypesController(IMapper mapper, IProductTypeService productTypeService, IPaginationService paginationService)
    {
        _mapper = mapper;
        _productTypeService = productTypeService;
        _paginationService = paginationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ProductTypeResponse>>> GetProductsAsync([FromQuery] BaseQueryParams productParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(productParams);
        var models = _productTypeService.GetAllAsync(query);
        var paged = await _paginationService.ApplyPaginationAsync(models, query.PageNumber, query.PageSize);
        var output = _mapper.Map<PagedList<ProductTypeResponse>>(paged);
        Response.AddPaginationHeader(output.Metadata);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductTypeResponse>> GetProductAsync(int id)
    {
        var result = await _productTypeService.GetAsync(id);
        return _mapper.Map<Result<ProductTypeResponse>>(result).ToActionResult();
    }
}
