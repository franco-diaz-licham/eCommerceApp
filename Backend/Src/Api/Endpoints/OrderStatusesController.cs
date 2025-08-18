namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class OrderStatusesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IOrderStatusService _OrderStatusService;

    public OrderStatusesController(IMapper mapper, IOrderStatusService OrderStatusService)
    {
        _OrderStatusService = OrderStatusService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<OrderStatusResponse>>> GetOrderStatussAsync([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = await _OrderStatusService.GetAllAsync(query);
        var output = _mapper.Map<PagedList<OrderStatusResponse>>(models);
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
