using Backend.Src.Infrastructure.Persistence;

namespace Backend.Src.Application.Services;

public class FilterService(DataContext db, IMapper mapper) : IFilterService
{
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _db = db;

    public async Task<ProductFiltersDto> GetProductFilters()
    {
        var brands = await _db.Products.Select(x => x.Brand).ProjectTo<BrandDto>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        var types = await _db.Products.Select(x => x.ProductType).ProjectTo<ProductTypeDto>(_mapper.ConfigurationProvider).Distinct().ToListAsync();
        return new(brands, types);
    }
}
