namespace Backend.Src.Application.Services;

public class BasketService(DataContext db, IMapper mapper, ILogger<BasketService> logger, IPaymentGateway payments) : IBasketService
{
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<BasketService> _logger = logger;
    private readonly DataContext _db = db;
    private readonly IPaymentGateway _payments = payments;

    public async Task<Result<BasketDto>> CreateBasketAsync()
    {
        // Create new Basket
        var basket = new BasketEntity();
        _db.Baskets.Add(basket);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<BasketDto>.Fail("Basket could not be created...", ResultTypeEnum.Invalid);
        return Result<BasketDto>.Success(_mapper.Map<BasketDto>(basket), ResultTypeEnum.Created);
    }

    public async Task<Result<BasketDto>> GetBasketAsync(int id)
    {
        var basket = await _db.Baskets.AsNoTracking().Where(b => b.Id == id).Include(b => b.BasketItems).ThenInclude(i => i.Product).ProjectTo<BasketDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        if(basket is null) return Result<BasketDto>.Fail("Basket not found...", ResultTypeEnum.NotFound);
        return Result<BasketDto>.Success(basket, ResultTypeEnum.Success);
    }

    public async Task<Result<BasketDto>> AddItemAsync(BasketItemCreateDto dto)
    {
        // Validate.
        if (dto.Quantity <= 0) return Result<BasketDto>.Fail("Quantity must be positive.", ResultTypeEnum.Invalid);
        var basket = await _db.Baskets.Include(b => b.BasketItems).SingleOrDefaultAsync(b => b.Id == dto.BasketId);
        if (basket is null) return Result<BasketDto>.Fail("Basket not found...", ResultTypeEnum.NotFound);
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null) return Result<BasketDto>.Fail("Product not found...", ResultTypeEnum.NotFound);

        // Add item.
        basket.AddItem(dto.ProductId, product.UnitPrice, dto.Quantity);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if(!saved) return Result<BasketDto>.Fail("Item could not be added...", ResultTypeEnum.Invalid);
        var output = _mapper.Map<BasketDto>(basket);
        return Result<BasketDto>.Success(output, ResultTypeEnum.Success);
    }

    public async Task<Result<bool>> RemoveItemAsync(BasketItemDto dto)
    {
        // Validate.
        var basket = await _db.Baskets.Include(b => b.BasketItems).SingleOrDefaultAsync(b => b.Id == dto.BasketId);
        if (basket is null) return Result<bool>.Fail("Basket not found...", ResultTypeEnum.NotFound);

        // Remove item.
        basket.RemoveItem(dto.ProductId);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<bool>.Fail("Item could not be removed...", ResultTypeEnum.Invalid);
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }

    public async Task<Result<BasketDto>> AddCouponAsync(BasketCouponDto dto)
    {
        // Get and validate
        if (string.IsNullOrWhiteSpace(dto.Code)) return Result<BasketDto>.Fail("Coupon code is required...", ResultTypeEnum.Invalid);
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).Include(b => b.Coupon).FirstOrDefaultAsync();
        if (basket == null || string.IsNullOrEmpty(basket.ClientSecret)) return Result<BasketDto>.Fail("Unable to apply voucher...", ResultTypeEnum.Invalid);

        // Get coupon info
        var couponInfo = await _payments.TryGetCouponByPromoCodeAsync(dto.Code);
        if (couponInfo is null) return Result<BasketDto>.Fail("Unable to apply coupon...", ResultTypeEnum.Invalid);
        var couponDto = _mapper.Map<CouponDto>(couponInfo);

        // Update basket info
        basket.AddCoupon(_mapper.Map<CouponEntity>(couponDto));
        var piModel = await _payments.CreateOrUpdateAsync(basket.TotalToMinorUnits(), "aud", basket.PaymentIntentId);
        basket.AttachPaymentIntent(piModel.IntentId, piModel.ClientSecret ?? "");

        // Save changes and output
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<BasketDto>.Fail("Coupon could not be added...", ResultTypeEnum.Invalid);
        var output = _mapper.Map<BasketDto>(basket);
        return Result<BasketDto>.Success(output, ResultTypeEnum.Success);
    }

    public async Task<Result<bool>> RemoveCouponAsync(BasketCouponDto dto)
    {
        // Validate
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).Include(b => b.Coupon).FirstOrDefaultAsync();
        if (basket == null || basket.Coupon is null) return Result<bool>.Fail("Unable to update basket with coupon...", ResultTypeEnum.Invalid);

        // Remove coupon
        basket.RemoveCoupon();

        // Save changes
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<bool>.Fail("Coupon could not be removed...", ResultTypeEnum.Invalid);
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }
}
