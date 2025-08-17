namespace Backend.Src.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public BrandService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public IQueryable<BrandDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Brands.AsNoTracking();
        var queryContext = new QueryStrategyContext<BrandEntity>(
            new SearchEvaluatorStrategy<BrandEntity>(specs.SearchTerm, new BrandSearchProvider()),
            new SortEvaluatorStrategy<BrandEntity>(specs.OrderBy, new BrandSortProvider())
        );
        return queryContext.ApplyQuery(query).ProjectTo<BrandDTO>(_mapper.ConfigurationProvider);
    }

    public async Task<Result<BrandDTO>> GetAsync(int id)
    {
        var dto = await _db.Brands.AsNoTracking().Where(p => p.Id == id).ProjectTo<BrandDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<BrandDTO>.Fail("Brand not found...", ResultTypeEnum.NotFound);
        return Result<BrandDTO>.Success(dto, ResultTypeEnum.Success);
    }
}
