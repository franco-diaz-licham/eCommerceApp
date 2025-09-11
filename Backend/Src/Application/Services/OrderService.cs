namespace Backend.Src.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public OrderService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<List<OrderDto>>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Orders.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderEntity>(
            new SearchEvaluatorStrategy<OrderEntity>(specs.SearchTerm, new OrderSearchProvider()),
            new SortEvaluatorStrategy<OrderEntity>(specs.OrderBy, new OrderSortProvider()),
            new SelectEvaluatorStrategy<OrderEntity>(specs.PageNumber, specs.PageSize)
        );
        var filtered = queryContext.ApplyQuery(query);
        var projected = await filtered.ProjectTo<OrderDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (projected is null) return Result<List<OrderDto>>.Fail("Orders not found...", ResultTypeEnum.NotFound);
        return Result<List<OrderDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<Result<OrderDto>> GetAsync(int id, string email)
    {
        var output = await _db.Orders.AsNoTracking().Where(o => o.UserEmail == email && o.Id == id).AsNoTracking().ProjectTo<OrderDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if(output is null) return Result<OrderDto>.Fail("Order could not be found...", ResultTypeEnum.Invalid);
        return Result<OrderDto>.Success(output, ResultTypeEnum.Created);
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto)
    {
        // Read associated basket and validate
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.BasketItems).ThenInclude(i => i.Product).Include(i => i.Coupon).FirstOrDefaultAsync();
        if (basket is null || basket.BasketItems.Count == 0) return Result<OrderDto>.Fail("Basket is empty.", ResultTypeEnum.Invalid);
        if (string.IsNullOrEmpty(basket.PaymentIntentId)) return Result<OrderDto>.Fail("Invalid basket payment intent.", ResultTypeEnum.Invalid);

        // Create order items from basket items
        var orderItems = new List<OrderItemEntity>();
        foreach (var bi in basket.BasketItems)
        {
            if (bi.Quantity <= 0) return Result<OrderDto>.Fail("An item has zero quantity.", ResultTypeEnum.Invalid);
            if (bi.Product is null) return Result<OrderDto>.Fail("Product not found.", ResultTypeEnum.Invalid);
            orderItems.Add(new OrderItemEntity(bi.ProductId, bi.Product.Name, bi.UnitPrice, bi.Quantity));
        }

        // Calculate discount
        var deliveryFee = 0m;
        if (basket.Subtotal < 100.00m && basket.Discount == 0m) deliveryFee = 5.00m;

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
                return Result<OrderDto>.Fail($"Insufficient stock for '{bi.Product?.Name ?? bi.ProductId.ToString()}'.", ResultTypeEnum.Invalid);
            }
        }

        // Update order
        var order = await _db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.PaymentIntentId == basket.PaymentIntentId);
        var shipping = new ShippingAddress(dto.ShippingAddress.Line1, dto.ShippingAddress.Line2, dto.ShippingAddress.City, dto.ShippingAddress.State, dto.ShippingAddress.PostalCode, dto.ShippingAddress.Country);
        var summary = _mapper.Map<PaymentSummary>(dto.PaymentSummary);
        if (order == null)
        {
            order = new OrderEntity(dto.UserEmail, shipping, basket.PaymentIntentId, deliveryFee, basket.Subtotal, basket.Discount, summary, orderItems);
            _db.Orders.Add(order);
        }
        else
        {
            orderItems.ForEach(x => order.AddItem(x.ProductId, x.ProductName, x.UnitPrice, x.Quantity));
            order.SetShippingAddress(shipping);
            order.UpdateCharges(deliveryFee, basket.Subtotal, basket.Discount);
            order.SetPaymentSummary(summary);
        }

        // Save and validate actions
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved)
        {
            await transaction.RollbackAsync();
            return Result<OrderDto>.Fail("Order could not be created.", ResultTypeEnum.Invalid);
        }

        await transaction.CommitAsync();

        // Return mapped order (not basket)
        var dtoOut = _mapper.Map<OrderDto>(order);
        return Result<OrderDto>.Success(dtoOut, ResultTypeEnum.Created);
    }
}
