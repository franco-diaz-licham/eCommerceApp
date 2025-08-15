namespace Backend.Src.Infrastructure.Services;

public class BasketService : IBasketService
{
    private readonly IMapper _mapper;
    private readonly ILogger<BasketService> _logger;
    private readonly DataContext _db;
    private readonly IPaymentService _paymentService;

    public BasketService(DataContext db, IMapper mapper, ILogger<BasketService> logger, IPaymentService paymentService)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task<BasketDTO?> CreateBasketAsync()
    {
        var basket = new BasketEntity();
        _db.Baskets.Add(basket);
        await _db.SaveChangesAsync();
        return _mapper.Map<BasketDTO>(basket);
    }

    public async Task<BasketDTO?> GetBasketAsync(int id)
    {
        var output = await _db.Baskets.Where(b => b.Id == id).Include(b => b.Items).ThenInclude(i => i.Product).ProjectTo<BasketDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        return output;
    }

    public async Task<BasketDTO?> AddItemAsync(BasketItemCreateDTO dto)
    {
        var basket = await _db.Baskets.FindAsync(dto.BasketId);
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null || basket is null) return default;

        basket.AddItem(dto.ProductId, product.UnitPrice, dto.Quantity);
        await _db.SaveChangesAsync();
        return _mapper.Map<BasketDTO>(basket);
    }

    public async Task<BasketDTO?> RemoveItemAsync(BasketItemCreateDTO dto)
    {
        var basket = await _db.Baskets.FindAsync(dto.BasketId);
        if (basket is null) return default;
        basket.RemoveItem(dto.ProductId, dto.Quantity);
        await _db.SaveChangesAsync();
        return _mapper.Map<BasketDTO>(basket);
    }

    public async Task<Result<BasketDTO>> AddCouponAsync(BasketCouponDTO dto)
    {
        // validate.
        if (string.IsNullOrWhiteSpace(dto.Code)) return Result<BasketDTO>.Fail("Coupon.Fail", "Coupon code is required...", ResultTypeEnum.Invalid);

        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.Items).Include(b => b.Coupon).FirstOrDefaultAsync();
        if (basket == null || string.IsNullOrEmpty(basket.ClientSecret)) return Result<BasketDTO>.Fail("Coupon.Fail", "Unable to apply voucher...", ResultTypeEnum.Invalid);
        var coupon = await _paymentService.GetCouponFromPromoCode(dto.Code);
        if (coupon == null) return Result<BasketDTO>.Fail("Coupon.Fail", "Invalid coupon....", ResultTypeEnum.Invalid);

        // update basket.
        basket.AddCoupon(coupon.CouponId, _mapper.Map<CouponEntity>(coupon));
        var intent = await _paymentService.CreateOrUpdatePaymentIntent(_mapper.Map<BasketDTO>(basket));
        if (intent == null) return Result<BasketDTO>.Fail("Coupon.Fail", "Problem applying coupon to basket...", ResultTypeEnum.Invalid);

        // saves changes to db.
        await _db.SaveChangesAsync();
        return Result<BasketDTO>.Success(_mapper.Map<BasketDTO>(basket), ResultTypeEnum.Success);
    }

    public async Task<Result<BasketDTO>> RemoveCouponAsync(BasketCouponDTO dto)
    {
        // validate
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.Items).Include(b => b.Coupon).FirstOrDefaultAsync();
        if (basket == null || basket.Coupon is null) return Result<BasketDTO>.Fail("Coupon.Fail", "Unable to update basket with coupon...", ResultTypeEnum.Invalid);
        
        // update basket
        var intent = await _paymentService.CreateOrUpdatePaymentIntent(_mapper.Map<BasketDTO>(basket), true);
        if (intent == null) return Result<BasketDTO>.Fail("Coupon.Fail", "Problem removing coupon from basket...", ResultTypeEnum.Invalid);

        // update database
        basket.RemoveCoupon();
        await _db.SaveChangesAsync();
        return Result<BasketDTO>.Success(_mapper.Map<BasketDTO>(basket), ResultTypeEnum.Accepted);
    }
}
