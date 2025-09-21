using Backend.Src.Infrastructure.Persistence;

namespace Backend.Src.Application.Services;

public class OrderStatusService(DataContext db, IMapper mapper) : IOrderStatusService
{
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _db = db;

    public async Task<Result<List<OrderStatusDto>>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.OrderStatuses.AsNoTracking();
        var queryContext = new QueryStrategyContext<OrderStatusEntity>(
            new SearchEvaluatorStrategy<OrderStatusEntity>(specs.SearchTerm, new OrderStatusSearchProvider()),
            new SortEvaluatorStrategy<OrderStatusEntity>(specs.OrderBy, new OrderStatusSortProvider()),
            new SelectEvaluatorStrategy<OrderStatusEntity>(specs.PageNumber, specs.PageSize)
        );
        var filtered = queryContext.ApplyQuery(query);
        var projected = await filtered.ProjectTo<OrderStatusDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (projected is null) return Result<List<OrderStatusDto>>.Fail("Order Status not found...", ResultTypeEnum.NotFound);
        return Result<List<OrderStatusDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<Result<OrderStatusDto>> GetAsync(int id)
    {
        var dto = await _db.OrderStatuses.AsNoTracking().Where(p => p.Id == id).ProjectTo<OrderStatusDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<OrderStatusDto>.Fail("Order Status not found...", ResultTypeEnum.NotFound);
        return Result<OrderStatusDto>.Success(dto, ResultTypeEnum.Success);
    }
}
