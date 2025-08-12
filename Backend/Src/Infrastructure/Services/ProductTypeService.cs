namespace Backend.Src.Infrastructure.Services;

public class ProductTypeService : IProductTypeService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public ProductTypeService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Method which gets all ProductTypes.
    /// </summary>
    public IQueryable<ProductTypeDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var baseQuery = _db.ProductTypes.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductTypeEntity>(
            new SearchEvaluatorStrategy<ProductTypeEntity>(specs.SearchTerm, new ProductTypeSearchProvider()),
            new SortEvaluatorStrategy<ProductTypeEntity>(specs.OrderBy, new ProductTypeSortProvider())
        );
 
        var query = queryContext.Execute(baseQuery);
        var output = query.ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider);
        return output;
    }

    /// <summary>
    /// Method which gets a ProductType.
    /// </summary>
    public async Task<ProductTypeDTO?> GetAsync(int id)
    {
        var output = await _db.ProductTypes
                                .AsNoTracking()
                                .Where(p => p.Id == id)
                                .ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider)
                                .SingleOrDefaultAsync();
        return output;
    }
}
