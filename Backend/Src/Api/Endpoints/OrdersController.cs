namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMapper mapper, IOrderService orderService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IOrderService _orderService = orderService;

    [HttpGet]
    public async Task<ActionResult<PagedList<OrderResponse>>> GetOrders([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = await _orderService.GetAllAsync(query);
        var output = _mapper.Map<PagedList<OrderResponse>>(models);
        Response.AddPaginationHeader(output.Metadata);
        return Ok(new ApiResponse(StatusCodes.Status200OK, output));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetOrderAsync(int id)
    {
        var result = await _orderService.GetAsync(id, User.GetUsername());
        return _mapper.Map<Result<OrderResponse>>(result).ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(_mapper.Map<OrderCreateDto>(request));
        return _mapper.Map<Result<OrderResponse>>(result).ToActionResult();
    }
}
