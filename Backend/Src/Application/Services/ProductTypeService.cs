namespace Backend.Src.Application.Services;

public class ProductTypeService(DataContext db, IMapper mapper) : IProductTypeService
{
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _db = db;

    public async Task<Result<List<ProductTypeDto>>> GetAllAsync(BaseQuerySpecs specs)
    {
        var query = _db.ProductTypes.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductTypeEntity>(
            new SearchEvaluatorStrategy<ProductTypeEntity>(specs.SearchTerm, new ProductTypeSearchProvider()),
            new SortEvaluatorStrategy<ProductTypeEntity>(specs.OrderBy, new ProductTypeSortProvider()),
            new SelectEvaluatorStrategy<ProductTypeEntity>(specs.PageNumber, specs.PageSize)
        );
        var filtered = queryContext.ApplyQuery(query);
        var projected = await filtered.ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (projected is null) return Result<List<ProductTypeDto>>.Fail("Product types not found...", ResultTypeEnum.NotFound);
        return Result<List<ProductTypeDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<Result<ProductTypeDto>> GetAsync(int id)
    {
        var dto = await _db.ProductTypes.AsNoTracking().Where(p => p.Id == id).ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        if (dto is null) return Result<ProductTypeDto>.Fail("Product type not found...", ResultTypeEnum.NotFound);
        return Result<ProductTypeDto>.Success(dto, ResultTypeEnum.Success);
    }
}
