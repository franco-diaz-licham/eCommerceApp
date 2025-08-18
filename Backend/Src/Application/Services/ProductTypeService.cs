namespace Backend.Src.Application.Services;

public class ProductTypeService : IProductTypeService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public ProductTypeService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

   
    public async Task<PagedList<ProductTypeDto>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.ProductTypes.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductTypeEntity>(
            new SearchEvaluatorStrategy<ProductTypeEntity>(specs.SearchTerm, new ProductTypeSearchProvider()),
            new SortEvaluatorStrategy<ProductTypeEntity>(specs.OrderBy, new ProductTypeSortProvider())
        );
        var filtered = queryContext.ApplyQuery(query);
        var count = await filtered.CountAsync();
        var projected = filtered.ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider);
        return await PagedList<ProductTypeDto>.ToPagedList(projected, count, specs.PageNumber, specs.PageSize);
    }

    public async Task<Result<ProductTypeDto>> GetAsync(int id)
    {
        var dto = await _db.ProductTypes.AsNoTracking().Where(p => p.Id == id).ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<ProductTypeDto>.Fail("Product type not found...", ResultTypeEnum.NotFound);
        return Result<ProductTypeDto>.Success(dto, ResultTypeEnum.Success);
    }
}
