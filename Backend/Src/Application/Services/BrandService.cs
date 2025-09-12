namespace Backend.Src.Application.Services;

public class BrandService(DataContext db, IMapper mapper) : IBrandService
{
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _db = db;

    public async Task<Result<List<BrandDto>>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.Brands.AsNoTracking();
        var queryContext = new QueryStrategyContext<BrandEntity>(
            new SearchEvaluatorStrategy<BrandEntity>(specs.SearchTerm, new BrandSearchProvider()),
            new SortEvaluatorStrategy<BrandEntity>(specs.OrderBy, new BrandSortProvider()),
            new SelectEvaluatorStrategy<BrandEntity>(specs.PageNumber, specs.PageSize)
        );
        var filtered = queryContext.ApplyQuery(query);
        var projected = await filtered.ProjectTo<BrandDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (projected is null) return Result<List<BrandDto>>.Fail("Brands not found...", ResultTypeEnum.NotFound);
        return Result<List<BrandDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<Result<BrandDto>> GetAsync(int id)
    {
        var dto = await _db.Brands.AsNoTracking().Where(p => p.Id == id).ProjectTo<BrandDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<BrandDto>.Fail("Brand not found...", ResultTypeEnum.NotFound);
        return Result<BrandDto>.Success(dto, ResultTypeEnum.Success);
    }
}
