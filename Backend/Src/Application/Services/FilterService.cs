namespace Backend.Src.Application.Services;

public class FilterService : IFilterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _db;

    public FilterService(DataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductFiltersDto> GetProductFilters()
    {
        var brands = await _db.Products.Select(x => x.Brand).ProjectTo<BrandDto>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        var types = await _db.Products.Select(x => x.ProductType).ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        return new(brands, types);
    }
}
