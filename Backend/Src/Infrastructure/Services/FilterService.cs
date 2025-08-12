namespace Backend.Src.Infrastructure.Services;

public class FilterService : IFilterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public FilterService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductFiltersDTO> GetProductFilters()
    {
        var brands = await _db.Products.Select(x => x.Brand).ProjectTo<BrandDTO>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        var types = await _db.Products.Select(x => x.ProductType).ProjectTo<ProductTypeDTO>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        return new(brands, types);
    }
}
