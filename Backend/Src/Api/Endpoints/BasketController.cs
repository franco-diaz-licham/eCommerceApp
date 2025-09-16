namespace Backend.Src.Api.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class BasketController(IBasketService basketService, IMapper mapper) : ControllerBase
{
    private readonly IBasketService _basketService = basketService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<ActionResult<BasketResponse>> CreateBasket()
    {
        var result = await _basketService.CreateBasketAsync();
        var location = Url.RouteUrl(nameof(GetBasket));
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult(location);
    }

    [HttpGet("{id:int}", Name = nameof(GetBasket))]
    public async Task<ActionResult<BasketResponse>> GetBasket(int id)
    {
        var result = await _basketService.GetBasketAsync(id);
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeleteBasketAsaync(int id)
    {
        var result = await _basketService.DeleteBasketAsync(id);
        return result.ToActionResult();
    }

    [HttpPost("add-item")]
    public async Task<ActionResult<BasketResponse>> AddBasketItemAsync(BasketItemAddRequest request)
    {
        var item = _mapper.Map<BasketItemCreateDto>(request);
        var result = await _basketService.AddItemAsync(item);
        var location = Url.RouteUrl(nameof(GetBasket));
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult(location);
    }

    [HttpDelete("remove-item")]
    public async Task<ActionResult> RemoveBasketItemAsync(BasketItemRemoveRequest request)
    {
        var item = _mapper.Map<BasketItemDto>(request);
        var result = await _basketService.RemoveItemAsync(item);
        return result.ToActionResult();
    }

    [HttpPost("add-coupon")]
    public async Task<ActionResult<BasketResponse>> AddCouponCodeAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDto>(request);
        var result = await _basketService.AddCouponAsync(coupon);
        var location = Url.RouteUrl(nameof(GetBasket));
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult(location);
    }

    [HttpDelete("remove-coupon")]
    public async Task<ActionResult> RemoveCouponAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDto>(request);
        var result = await _basketService.RemoveCouponAsync(coupon);
        return result.ToActionResult();
    }
}
