namespace Backend.Src.Application.Services;

public class OrderService(DataContext db, IMapper mapper, ICurrentUser currentUser) : IOrderService
{
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _db = db;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<List<OrderDto>>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Orders.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderEntity>(
            new SearchEvaluatorStrategy<OrderEntity>(specs.SearchTerm, new OrderSearchProvider()),
            new SortEvaluatorStrategy<OrderEntity>(specs.OrderBy, new OrderSortProvider()),
            new SelectEvaluatorStrategy<OrderEntity>(specs.PageNumber, specs.PageSize)
        );
        var filtered = queryContext.ApplyQuery(query).Where(x => x.UserId == _currentUser.UserId);
        var projected = await filtered.ProjectTo<OrderDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (!projected.Any()) return Result<List<OrderDto>>.Fail("Orders not found...", ResultTypeEnum.NotFound);
        return Result<List<OrderDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<Result<OrderDto>> GetAsync(int id)
    {
        var output = await _db.Orders.AsNoTracking().Where(o => o.UserId == _currentUser.UserId && o.Id == id).AsNoTracking().ProjectTo<OrderDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if(output is null) return Result<OrderDto>.Fail("Order could not be found...", ResultTypeEnum.Invalid);
        return Result<OrderDto>.Success(output, ResultTypeEnum.Created);
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto)
    {
        // Validate
        var basket = await _db.Baskets
                        .Where(b => b.Id == dto.BasketId)
                        .Include(b => b.BasketItems).ThenInclude(i => i.Product).ThenInclude(x => x.Photo)
                        .Include(i => i.Coupon)
                        .FirstOrDefaultAsync();
        if (basket is null || basket.BasketItems.Count == 0) return Result<OrderDto>.Fail("Basket is empty.", ResultTypeEnum.Invalid);
        if (string.IsNullOrEmpty(basket.PaymentIntentId)) return Result<OrderDto>.Fail("Invalid basket payment intent.", ResultTypeEnum.Invalid);
        if(_currentUser.UserId is null) return Result<OrderDto>.Fail("User not found.", ResultTypeEnum.Invalid);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Create items from basket items and apply atomic concurrent handle: allow SQL handle competting update requests
        var orderItems = new List<OrderItemEntity>();
        foreach (var bi in basket.BasketItems)
        {
                orderItems.Add(new OrderItemEntity(bi.ProductId, bi.Product!.Name, bi.UnitPrice, bi.Quantity));
                var affected = await _db.Products
                                    .Where(p => p.Id == bi.ProductId && p.QuantityInStock >= bi.Quantity)
                                    .ExecuteUpdateAsync(setters => setters
                                    .SetProperty(p => p.QuantityInStock, p => p.QuantityInStock - bi.Quantity));

            if (affected == 0)
            {
                await transaction.RollbackAsync();
                return Result<OrderDto>.Fail($"There was a problem creating the order.", ResultTypeEnum.Invalid);
            }
        }

        // Create order and delete basket
        var address = _mapper.Map<ShippingAddress>(dto.ShippingAddress);
        var summary = _mapper.Map<PaymentSummary>(dto.PaymentSummary);
        var order = new OrderEntity(_currentUser.UserId, address, basket.PaymentIntentId, basket.DeliveryFee, basket.Subtotal, basket.Discount, summary, orderItems);
        _db.Orders.Add(order);
        _db.Baskets.Remove(basket);

        // Save and validate actions
        var saved = await _db.SaveChangesAsync() > 0;
        if (!saved)
        {
            await transaction.RollbackAsync();
            return Result<OrderDto>.Fail("Order could not be created.", ResultTypeEnum.Invalid);
        }

        // Map and return
        await transaction.CommitAsync();
        var dtoOut = _mapper.Map<OrderDto>(order);
        return Result<OrderDto>.Success(dtoOut, ResultTypeEnum.Created);
    }
}
