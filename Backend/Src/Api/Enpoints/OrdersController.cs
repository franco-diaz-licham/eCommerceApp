namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;
    private readonly IPaginationService _paginationService;

    public OrdersController(IMapper mapper, IOrderService orderService, IPaginationService paginsationService)
    {
        _mapper = mapper;
        _orderService = orderService;
        _paginationService = paginsationService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<OrderResponse>>> GetOrders([FromQuery] BaseQueryParams queryParams)
    {
        var query = _mapper.Map<BaseQuerySpecs>(queryParams);
        var models = _orderService.GetAllAsync(query);
        var paged = await _paginationService.ApplyPaginationAsync(models, query.PageNumber, query.PageSize);
        var output = _mapper.Map<PagedList<OrderResponse>>(paged);
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
        var result = await _orderService.CreateOrderAsync(_mapper.Map<OrderCreateDTO>(request));
        return _mapper.Map<Result<OrderResponse>>(result).ToActionResult();
    }
}
