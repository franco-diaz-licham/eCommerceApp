namespace Backend.Src.Infrastructure.Services;

public class OrderService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;
    private readonly IPaymentService _paymentService;

    public OrderService(DataContext db, IMapper mapper, IPaymentService paymentService)
    {
        _db = db;
        _mapper = mapper;
        _paymentService = paymentService;
    }

    public IQueryable<OrderDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var baseQuery = _db.Orders.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderEntity>(
            new SearchEvaluatorStrategy<OrderEntity>(specs.SearchTerm, new OrderSearchProvider()),
            new SortEvaluatorStrategy<OrderEntity>(specs.OrderBy, new OrderSortProvider())
        );
        var query = queryContext.Execute(baseQuery);
        var output = query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider);
        return output;
    }

    public async Task<OrderDTO?> GetAsync(int id, int userId)
    {
        var output = await _db.Orders.AsNoTracking().Where(p => p.UserId == userId && p.Id == id).ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        return output;
    }

    //public async Task<Result<OrderDTO>> CreateOrderAsync(OrderCreateDTO dto)
    //{
    //    var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.Items).ThenInclude(i => i.Product).AsNoTracking().FirstOrDefaultAsync();
    //    if (basket == null || basket.Items.Count == 0 || string.IsNullOrEmpty(basket.PaymentIntentId)) return Result<OrderDTO>.Fail("Coupon.Fail", "Coupon code is required...", ResultTypeEnum.Invalid);

    //    var shipping = new ShippingAddress(dto.ShippingAddress.Line1, dto.ShippingAddress.Line2, dto.ShippingAddress.City, dto.ShippingAddress.State, dto.ShippingAddress.PostalCode, dto.ShippingAddress.Country);

    //    var order = new OrderEntity(dto.BuyerEmail, shipping, dto.PaymentIntentId);
    //    order.SetDeliveryFee(dto.DeliveryFee);
    //    if (dto.Discount > 0) order.ApplyDiscount(dto.Discount);

    //    foreach (var it in dto.Items)
    //        order.AddItem(it.ProductId, it.Name, it.UnitPrice, it.Quantity);

    //    _db.Orders.Add(order);
    //    await _db.SaveChangesAsync();

    //}
}
