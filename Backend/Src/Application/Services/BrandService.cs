namespace Backend.Src.Application.Services;

public class BrandService : IBrandService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public BrandService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedList<BrandDto>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Brands.AsNoTracking();
        var queryContext = new QueryStrategyContext<BrandEntity>(
            new SearchEvaluatorStrategy<BrandEntity>(specs.SearchTerm, new BrandSearchProvider()),
            new SortEvaluatorStrategy<BrandEntity>(specs.OrderBy, new BrandSortProvider())
        );
        var filtered = queryContext.ApplyQuery(query);
        var count = await filtered.CountAsync();
        var projected = filtered.ProjectTo<BrandDto>(_mapper.ConfigurationProvider);
        return await PagedList<BrandDto>.ToPagedList(projected, count, specs.PageNumber, specs.PageSize);
    }

    public async Task<Result<BrandDto>> GetAsync(int id)
    {
        var dto = await _db.Brands.AsNoTracking().Where(p => p.Id == id).ProjectTo<BrandDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<BrandDto>.Fail("Brand not found...", ResultTypeEnum.NotFound);
        return Result<BrandDto>.Success(dto, ResultTypeEnum.Success);
    }
}
