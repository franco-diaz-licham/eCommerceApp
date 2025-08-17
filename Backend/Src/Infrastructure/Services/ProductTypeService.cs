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

   
    public IQueryable<ProductTypeDTO> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.ProductTypes.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductTypeEntity>(
            new SearchEvaluatorStrategy<ProductTypeEntity>(specs.SearchTerm, new ProductTypeSearchProvider()),
            new SortEvaluatorStrategy<ProductTypeEntity>(specs.OrderBy, new ProductTypeSortProvider())
        );
        return queryContext.ApplyQuery(query).ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider);
    }

    public async Task<Result<ProductTypeDTO>> GetAsync(int id)
    {
        var dto = await _db.ProductTypes.AsNoTracking().Where(p => p.Id == id).ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<ProductTypeDTO>.Fail("Product type not found...", ResultTypeEnum.NotFound);
        return Result<ProductTypeDTO>.Success(dto, ResultTypeEnum.Success);
    }
}
