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
        var result = await _basketService.CreateBasketAsync();
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BasketResponse>> GetBasket(int id)
    {
        var result = await _basketService.GetBasketAsync(id);
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpPost("add-item")]
    public async Task<ActionResult<BasketResponse>> AddBasketItemAsync(BasketItemAddRequest request)
    {
        var item = _mapper.Map<BasketItemCreateDTO>(request);
        var result = await _basketService.AddItemAsync(item);
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpDelete("remove-item")]
    public async Task<ActionResult> RemoveBasketItemAsync(BasketItemRemoveRequest request)
    {
        var item = _mapper.Map<BasketItemCreateDTO>(request);
        var result = await _basketService.RemoveItemAsync(item);
        return result.ToActionResult();
    }

    [HttpPost("add-coupon")]
    public async Task<ActionResult<BasketResponse>> AddCouponCodeAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDTO>(request);
        var result = await _basketService.AddCouponAsync(coupon);
        return _mapper.Map<Result<BasketResponse>>(result).ToActionResult();
    }

    [HttpDelete("remove-coupon")]
    public async Task<ActionResult> RemoveCouponAsync(BasketCouponRequest request)
    {
        var coupon = _mapper.Map<BasketCouponDTO>(request);
        var result = await _basketService.RemoveCouponAsync(coupon);
        return result.ToActionResult();
    }
}
