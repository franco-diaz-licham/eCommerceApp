namespace Backend.Src.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;
    private readonly IRemotePaymentService _paymentService;

    public OrderService(DataContext db, IMapper mapper, IRemotePaymentService paymentService)
    {
        _db = db;
        _mapper = mapper;
        _paymentService = paymentService;
    }

    public IQueryable<OrderDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Orders.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderEntity>(
            new SearchEvaluatorStrategy<OrderEntity>(specs.SearchTerm, new OrderSearchProvider()),
            new SortEvaluatorStrategy<OrderEntity>(specs.OrderBy, new OrderSortProvider())
        );
        return queryContext.ApplyQuery(query).ProjectTo<OrderDTO>(_mapper.ConfigurationProvider);
    }

    public async Task<OrderDTO?> GetAsync(int id, string email)
    {
        var output = await _db.Orders.AsNoTracking().Where(p => p.UserEmail == email && p.Id == id).AsNoTracking().ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        return output;
    }

    public async Task<Result<OrderDTO>> CreateOrderAsync(OrderCreateDTO dto)
    {
        // Read associated basket and validate
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).ThenInclude(i => i.Product).Include(i => i.Coupon).FirstOrDefaultAsync();
        if (basket is null || basket.BasketItems.Count == 0) return Result<OrderDTO>.Fail("Basket is empty.", ResultTypeEnum.Invalid);
        if (string.IsNullOrEmpty(basket.PaymentIntentId)) return Result<OrderDTO>.Fail("Invalid basket payment intent.", ResultTypeEnum.Invalid);

        // Create order items from basket items
        var orderItems = new List<OrderItemEntity>();
        foreach (var bi in basket.BasketItems)
        {
            if (bi.Quantity <= 0) return Result<OrderDTO>.Fail("An item has zero quantity.", ResultTypeEnum.Invalid);
            if (bi.Product is null) return Result<OrderDTO>.Fail("Product not found.", ResultTypeEnum.Invalid);
            orderItems.Add(new OrderItemEntity(bi.ProductId, bi.Product.Name, bi.UnitPrice, bi.Quantity));
        }

        // Calculate discount
        var deliveryFee = basket.Subtotal > 100.00m ? 0 : 5.00m;
        var discount = 0m;
        if (basket.Coupon?.RemoteId is not null) discount = await _paymentService.CalculateDiscountFromAmount(basket.Coupon.RemoteId, basket.Subtotal);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Atomic concurrent handle: allow SQL handle competting update requests
        foreach (var bi in basket.BasketItems)
        {
            var affected = await _db.Products
                                    .Where(p => p.Id == bi.ProductId && p.QuantityInStock >= bi.Quantity)
                                    .ExecuteUpdateAsync(setters => setters
                                    .SetProperty(p => p.QuantityInStock, p => p.QuantityInStock - bi.Quantity));

            if (affected == 0)
            {
                await transaction.RollbackAsync();
                return Result<OrderDTO>.Fail($"Insufficient stock for '{bi.Product?.Name ?? bi.ProductId.ToString()}'.", ResultTypeEnum.Invalid);
            }
        }

        // Update order
        var order = await _db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.PaymentIntentId == basket.PaymentIntentId);
        var shipping = new ShippingAddress(dto.ShippingAddress.Line1, dto.ShippingAddress.Line2, dto.ShippingAddress.City, dto.ShippingAddress.State, dto.ShippingAddress.PostalCode, dto.ShippingAddress.Country);
        var summary = _mapper.Map<PaymentSummary>(dto.PaymentSummary);
        if (order == null)
        {
            order = new OrderEntity(dto.UserEmail, shipping, basket.PaymentIntentId, deliveryFee, basket.Subtotal, discount, summary, orderItems);
            _db.Orders.Add(order);
        }
        else
        {
            orderItems.ForEach(x => order.AddItem(x.ProductId, x.ProductName, x.UnitPrice, x.Quantity));
            order.SetShippingAddress(shipping);
            order.UpdateCharges(deliveryFee, basket.Subtotal, discount);
            order.SetPaymentSummary(summary);
        }

        // Save and validate actions
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved)
        {
            await transaction.RollbackAsync();
            return Result<OrderDTO>.Fail("Order could not be created.", ResultTypeEnum.Invalid);
        }

        await transaction.CommitAsync();

        // Return mapped order (not basket)
        var dtoOut = _mapper.Map<OrderDTO>(order);
        return Result<OrderDTO>.Success(dtoOut, ResultTypeEnum.Created);
    }
}
