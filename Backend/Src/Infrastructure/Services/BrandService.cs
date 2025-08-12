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

    /// <summary>
    /// Method which gets all Brands.
    /// </summary>
    public IQueryable<BrandDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var baseQuery = _db.Brands.AsNoTracking();
        var queryContext = new QueryStrategyContext<BrandEntity>(
            new SearchEvaluatorStrategy<BrandEntity>(specs.SearchTerm, new BrandSearchProvider()),
            new SortEvaluatorStrategy<BrandEntity>(specs.OrderBy, new BrandSortProvider())
        );

        var query = queryContext.Execute(baseQuery);
        var output = query.ProjectTo<BrandDTO>(_mapper.ConfigurationProvider);
        return output;
    }

    /// <summary>
    /// Method which gets a Brand.
    /// </summary>
    public async Task<BrandDTO?> GetAsync(int id)
    {
        var output = await _db.Brands
                            .AsNoTracking()
                            .Where(p => p.Id == id)
                            .ProjectTo<BrandDTO>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
        return output;
    }
}
