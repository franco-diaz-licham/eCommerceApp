namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class ProductTypesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProductTypeService _productTypeService;

    public ProductTypesController(IMapper mapper, IProductTypeService productTypeService)
    {
        _mapper = mapper;
        _productTypeService = productTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ProductTypeResponse>>> GetProductsAsync([FromQuery] BaseQueryParams productParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(productParams);
        var models = await _productTypeService.GetAllAsync(query);
        var output = _mapper.Map<PagedList<ProductTypeResponse>>(models);
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
