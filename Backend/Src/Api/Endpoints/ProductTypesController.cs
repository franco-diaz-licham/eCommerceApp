namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
[Tags("Product Types")]
public class ProductTypesController(IMapper mapper, IProductTypeService productTypeService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IProductTypeService _productTypeService = productTypeService;

    [HttpGet]
    public async Task<ActionResult<PagedList<ProductTypeResponse>>> GetProductsAsync([FromQuery] BaseQueryParams productParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(productParams);
        var models = await _productTypeService.GetAllAsync(query);
        var response = _mapper.Map<Result<List<ProductTypeResponse>>>(models);
        var output = response.ToActionPaginatedResult(Response, query.PageNumber, query.PageSize, response.TotalCount);
        return output;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductTypeResponse>> GetProductAsync(int id)
    {
        var result = await _productTypeService.GetAsync(id);
        return _mapper.Map<Result<ProductTypeResponse>>(result).ToActionResult();
    }
}
