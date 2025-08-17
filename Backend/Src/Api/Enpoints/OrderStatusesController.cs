namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class OrderStatusesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IOrderStatusService _OrderStatusService;
    private readonly IPaginationService _paginationService;

    public OrderStatusesController(IMapper mapper, IOrderStatusService OrderStatusService, IPaginationService paginationService)
    {
        _OrderStatusService = OrderStatusService;
        _paginationService = paginationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<OrderStatusResponse>>> GetOrderStatussAsync([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = _OrderStatusService.GetAllAsync(query);
        var paged = await _paginationService.ApplyPaginationAsync(models, query.PageNumber, query.PageSize);
        var output = _mapper.Map<PagedList<OrderStatusResponse>>(paged);
        Response.AddPaginationHeader(output.Metadata);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderStatusResponse>> GetOrderStatusAsync(int id)
    {
        var result = await _OrderStatusService.GetAsync(id);
        return _mapper.Map<Result<OrderStatusResponse>>(result).ToActionResult();
    }
}
