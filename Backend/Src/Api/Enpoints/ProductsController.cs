namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProductService _productService;
    private readonly IPaginationService _paginationService;
    private readonly IFilterService _filteService;

    public ProductsController(IMapper mapper, IProductService productService, IPaginationService paginationService, IFilterService filterService)
    {
        _productService = productService;
        _paginationService = paginationService;
        _mapper = mapper;
        _filteService = filterService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ProductResponse>>> GetProductsAsync([FromQuery] ProductQueryParams queryParams)
    {
        var query = _mapper.Map<ProductQuerySpecs>(queryParams);
        var models = _productService.GetAllAsync(query);
        var paged = await _paginationService.ApplyPaginationAsync(models, query.PageNumber, query.PageSize);
        var output = _mapper.Map<PagedList<ProductResponse>>(paged);
        Response.AddPaginationHeader(output.Metadata);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("{id:int}")]
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
    public async Task<ActionResult<ProductResponse>> CreateProductAsync(CreateProductRequest model)
    {
        var product = _mapper.Map<ProductCreateDTO>(model);
        var output = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetProductAsync), new { output.Id }, output);
    }

    [HttpPut]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(UpdateProductRequest model)
    {
        var product = _mapper.Map<ProductUpdateDTO>(model);
        var output = await _productService.UpdateAsync(product);
        return Accepted(new ApiResponse(202, output));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (result is false) return BadRequest(new ApiResponse(400));
        return NoContent();
    }
}
