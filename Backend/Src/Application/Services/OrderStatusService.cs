namespace Backend.Src.Application.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public OrderStatusService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedList<OrderStatusDto>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.OrderStatuses.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderStatusEntity>(
            new SearchEvaluatorStrategy<OrderStatusEntity>(specs.SearchTerm, new OrderStatusSearchProvider()),
            new SortEvaluatorStrategy<OrderStatusEntity>(specs.OrderBy, new OrderStatusSortProvider())
        );
        var filtered = queryContext.ApplyQuery(query);
        var count = await filtered.CountAsync();
        var projected = filtered.ProjectTo<OrderStatusDto>(_mapper.ConfigurationProvider);
        return await PagedList<OrderStatusDto>.ToPagedList(projected, count, specs.PageNumber, specs.PageSize);
    }

    public async Task<Result<OrderStatusDto>> GetAsync(int id)
    {
        var dto = await _db.OrderStatuses.AsNoTracking().Where(p => p.Id == id).ProjectTo<OrderStatusDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<OrderStatusDto>.Fail("Order Status not found...", ResultTypeEnum.NotFound);
        return Result<OrderStatusDto>.Success(dto, ResultTypeEnum.Success);
    }
}
