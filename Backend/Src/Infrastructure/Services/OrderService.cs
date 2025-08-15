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

    public async Task<OrderDTO?> GetAsync(int id, string userEmail)
    {
        var output = await _db.Orders
                            .AsNoTracking()
                            .Where(p => p.BuyerEmail == userEmail && p.Id == id)
                            .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
        return output;
    }

    public async Task<Result<OrderDTO>> CreateOrderAsync(OrderCreateDTO dto)
    {
        var basket = await _db.Baskets.Where(b => b.Id == dto.BasketId).Include(b => b.Items).ThenInclude(i => i.Product).AsNoTracking().FirstOrDefaultAsync();
        if (basket == null || basket.Items.Count == 0 || string.IsNullOrEmpty(basket.PaymentIntentId)) return Result<OrderDTO>.Fail("Coupon.Fail", "Coupon code is required...", ResultTypeEnum.Invalid);


        foreach (var item in basket.Items)
        {
            if (item.Product is null || item.Product.QuantityInStock < item.Quantity)  return null;

            var orderItem = new OrderItemEntity
            {
                ItemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    PictureUrl = item.Product.PictureUrl,
                    Name = item.Product.Name
                },
                Price = item.Product.Price,
                Quantity = item.Quantity
            };
            orderItems.Add(orderItem);

            item.Product.QuantityInStock -= item.Quantity;
        }

        return orderItems;



        var items = CreateOrderItems(basket.Items);

        if (items == null) return BadRequest("Some items out of stock");

    }
}
