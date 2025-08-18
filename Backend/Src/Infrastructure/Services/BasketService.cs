namespace Backend.Src.Infrastructure.Services;

public class BasketService : IBasketService
{
    private readonly IMapper _mapper;
    private readonly ILogger<BasketService> _logger;
    private readonly DataContext _db;
    private readonly IRemotePaymentService _paymentService;

    public BasketService(DataContext db, IMapper mapper, ILogger<BasketService> logger, IRemotePaymentService paymentService)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task<Result<BasketDTO>> CreateBasketAsync()
    {
        // Create new Basket
        var basket = new BasketEntity();
        _db.Baskets.Add(basket);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<BasketDTO>.Fail("Basket could not be created...", ResultTypeEnum.Invalid);
        return Result<BasketDTO>.Success(_mapper.Map<BasketDTO>(basket), ResultTypeEnum.Created);
    }

    public async Task<Result<BasketDTO>> GetBasketAsync(int id)
    {
        var basket = await _db.Baskets.AsNoTracking().Where(b => b.Id == id).Include(b => b.BasketItems).ThenInclude(i => i.Product).ProjectTo<BasketDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        if(basket is null) return Result<BasketDTO>.Fail("Basket not found...", ResultTypeEnum.NotFound);
        return Result<BasketDTO>.Success(basket, ResultTypeEnum.Success);
    }

    public async Task<Result<BasketDTO>> AddItemAsync(BasketItemCreateDTO dto)
    {
        // Validate.
        if (dto.Quantity <= 0) return Result<BasketDTO>.Fail("Quantity must be positive.", ResultTypeEnum.Invalid);
        var basket = await _db.Baskets.Include(b => b.BasketItems).SingleOrDefaultAsync(b => b.Id == dto.BasketId);
        if (basket is null) return Result<BasketDTO>.Fail("Basket not found...", ResultTypeEnum.NotFound);
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null) return Result<BasketDTO>.Fail("Product not found...", ResultTypeEnum.NotFound);

        // Add item.
        basket.AddItem(dto.ProductId, product.UnitPrice, dto.Quantity);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if(!saved) return Result<BasketDTO>.Fail("Item could not be added...", ResultTypeEnum.Invalid);
        var output = _mapper.Map<BasketDTO>(basket);
        return Result<BasketDTO>.Success(output, ResultTypeEnum.Success);
    }

    public async Task<Result<bool>> RemoveItemAsync(BasketItemCreateDTO dto)
    {
        // Validate.
        var basket = await _db.Baskets.Include(b => b.BasketItems).SingleOrDefaultAsync(b => b.Id == dto.BasketId);
        if (basket is null) return Result<bool>.Fail("Basket not found...", ResultTypeEnum.NotFound);

        // Remove item.
        basket.RemoveItem(dto.ProductId, dto.Quantity);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<bool>.Fail("Item could not be removed...", ResultTypeEnum.Invalid);
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }

    public async Task<Result<BasketDTO>> AddCouponAsync(BasketCouponDTO dto)
    {
        // Validate.
        if (string.IsNullOrWhiteSpace(dto.Code)) return Result<BasketDTO>.Fail("Coupon code is required...", ResultTypeEnum.Invalid);
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).Include(b => b.Coupon).FirstOrDefaultAsync();
        var coupon = await _paymentService.GetCouponFromPromoCode(dto.Code);
        if (basket == null || string.IsNullOrEmpty(basket.ClientSecret)) return Result<BasketDTO>.Fail("Unable to apply voucher...", ResultTypeEnum.Invalid);
        if (coupon == null) return Result<BasketDTO>.Fail("Invalid coupon....", ResultTypeEnum.Invalid);

        // Update basket.
        basket.AddCoupon(_mapper.Map<CouponEntity>(coupon));
        var (intent, clientSecret) = await _paymentService.CreateOrUpdatePaymentIntent(_mapper.Map<BasketDTO>(basket));
        basket.AttachPaymentIntent(intent, clientSecret);

        // Save changes to db.
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<BasketDTO>.Fail("Coupon could not be added...", ResultTypeEnum.Invalid);
        var output = _mapper.Map<BasketDTO>(basket);
        return Result<BasketDTO>.Success(output, ResultTypeEnum.Success);
    }

    public async Task<Result<bool>> RemoveCouponAsync(BasketCouponDTO dto)
    {
        // validate
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).Include(b => b.Coupon).FirstOrDefaultAsync();
        if (basket == null || basket.Coupon is null) return Result<bool>.Fail("Unable to update basket with coupon...", ResultTypeEnum.Invalid);
        
        // update basket
        await _paymentService.CreateOrUpdatePaymentIntent(_mapper.Map<BasketDTO>(basket), true);
        basket.RemoveCoupon();

        // update database
        basket.RemoveCoupon();
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved) return Result<bool>.Fail("Coupon could not be removed...", ResultTypeEnum.Invalid);
        return Result<bool>.Success(ResultTypeEnum.Accepted);
    }
}
