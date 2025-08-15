namespace Backend.Src.Api.Enpoints;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(IBasketService basketService, IMapper mapper)
    {
        _basketService = basketService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<BasketResponse>> CreateBasket()
    {
        var basket = await _basketService.CreateBasketAsync();
        if (basket == null) return NoContent();
        return _mapper.Map<BasketResponse>(basket);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BasketResponse>> GetBasket(int id)
    {
        var basket = await _basketService.GetBasketAsync(id);
        if (basket == null) return NoContent();
        return _mapper.Map<BasketResponse>(basket);
    }

    [HttpPost("add-item")]
    public async Task<ActionResult<BasketResponse>> AddBasketItemAsync(BasketItemAddRequest request)
    {
        var item = _mapper.Map<BasketItemCreateDTO>(request);
        var output = await _basketService.AddItemAsync(item);
        return Accepted(new ApiResponse(202));
    }

    [HttpDelete("remove-item")]
    public async Task<ActionResult> RemoveBasketItemAsync(BasketItemRemoveRequest request)
    {
        var item = _mapper.Map<BasketItemCreateDTO>(request);
        var output = await _basketService.RemoveItemAsync(item);
        return Accepted(new ApiResponse(202));
    }

    [HttpPost("add-coupon/{code}")]
    public async Task<ActionResult<BasketResponse>> AddCouponCodeAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDTO>(request);
        var result = await _basketService.AddCouponAsync(coupon);
        var output = Result<BasketResponse>.Success(_mapper.Map<BasketResponse>(result.Value), ResultTypeEnum.Accepted);
        return output.ToActionResult();
    }

    [HttpDelete("remove-coupon")]
    public async Task<ActionResult> RemoveCouponAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDTO>(request);
        var result = await _basketService.RemoveCouponAsync(coupon);
        var output = Result<BasketResponse>.Success(_mapper.Map<BasketResponse>(result.Value), ResultTypeEnum.Accepted);
        return output.ToActionResult();
    }
}
