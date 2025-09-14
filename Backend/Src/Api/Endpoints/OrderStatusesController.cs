namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
[Tags("Order Status")]
public class OrderStatusesController(IMapper mapper, IOrderStatusService OrderStatusService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IOrderStatusService _OrderStatusService = OrderStatusService;

    [HttpGet]
    public async Task<ActionResult<PagedList<OrderStatusResponse>>> GetOrderStatussAsync([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = await _OrderStatusService.GetAllAsync(query);
        var response = _mapper.Map<Result<List<OrderStatusResponse>>>(models);
        var output = response.ToActionPaginatedResult(Response, query.PageNumber, query.PageSize, response.TotalCount);
        return output;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderStatusResponse>> GetOrderStatusAsync(int id)
    {
        var result = await _OrderStatusService.GetAsync(id);
        return _mapper.Map<Result<OrderStatusResponse>>(result).ToActionResult();
    }
}
