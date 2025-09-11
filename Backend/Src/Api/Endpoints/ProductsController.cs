namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMapper mapper, IProductService productService, IFilterService filterService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IProductService _productService = productService;
    private readonly IFilterService _filteService = filterService;

    [HttpGet]
    public async Task<ActionResult<PagedList<ProductResponse>>> GetProductsAsync([FromQuery] ProductQueryParams queryParams)
    {
        var query = _mapper.Map<ProductQuerySpecs>(queryParams);
        var models = await _productService.GetAllAsync(query);
        var response = _mapper.Map<Result<List<ProductResponse>>>(models);
        var output = response.ToActionPaginatedResult(Response, query.PageNumber, query.PageSize, response.TotalCount);
        return output;
    }

    [HttpGet("{id:int}", Name = nameof(GetProductAsync))]
    public async Task<ActionResult<ProductResponse>> GetProductAsync(int id)
    {
        var product = await _productService.GetAsync(id);
        if (product == null) return NotFound();
        var output = _mapper.Map<ProductResponse>(product);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        var output = await _filteService.GetProductFilters();
        return Ok(output);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProductAsync([FromForm] CreateProductRequest model)
    {
        var product = _mapper.Map<ProductCreateDto>(model);
        var output = await _productService.CreateAsync(product);
        return CreatedAtRoute(nameof(GetProductAsync), new { id = output.Id }, output);
    }

    [HttpPut]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromForm] UpdateProductRequest model)
    {
        var product = _mapper.Map<ProductUpdateDto>(model);
        var output = await _productService.UpdateAsync(product);
        return Ok(new ApiResponse(200, output));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (result is false) return BadRequest(new ApiResponse(400));
        return NoContent();
    }
}
