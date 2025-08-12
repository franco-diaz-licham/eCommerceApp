namespace Backend.Src.Infrastructure.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public OrderStatusService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Method which gets all OrderStatuss.
    /// </summary>
    public IQueryable<OrderStatusDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var baseQuery = _db.OrderStatuses.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderStatusEntity>(
            new SearchEvaluatorStrategy<OrderStatusEntity>(specs.SearchTerm, new OrderStatusSearchProvider()),
            new SortEvaluatorStrategy<OrderStatusEntity>(specs.OrderBy, new OrderStatusSortProvider())
        );

        var query = queryContext.Execute(baseQuery);
        var output = query.ProjectTo<OrderStatusDTO>(_mapper.ConfigurationProvider);
        return output;
    }

    /// <summary>
    /// Method which gets a OrderStatus.
    /// </summary>
    public async Task<OrderStatusDTO?> GetAsync(int id)
    {
        var output = await _db.OrderStatuses
                            .AsNoTracking()
                            .Where(p => p.Id == id)
                            .ProjectTo<OrderStatusDTO>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
        return output;
    }
}
