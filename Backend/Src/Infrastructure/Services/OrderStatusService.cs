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

    public IQueryable<OrderStatusDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.OrderStatuses.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderStatusEntity>(
            new SearchEvaluatorStrategy<OrderStatusEntity>(specs.SearchTerm, new OrderStatusSearchProvider()),
            new SortEvaluatorStrategy<OrderStatusEntity>(specs.OrderBy, new OrderStatusSortProvider())
        );

        return queryContext.ApplyQuery(query).ProjectTo<OrderStatusDTO>(_mapper.ConfigurationProvider);
    }

    public async Task<Result<OrderStatusDTO>> GetAsync(int id)
    {
        var dto = await _db.OrderStatuses.AsNoTracking().Where(p => p.Id == id).ProjectTo<OrderStatusDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<OrderStatusDTO>.Fail("Order Status not found...", ResultTypeEnum.NotFound);
        return Result<OrderStatusDTO>.Success(dto, ResultTypeEnum.Success);
    }
}
