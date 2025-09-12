namespace Backend.Src.Application.Services;

public class ProductService(DataContext db, IMapper mapper, IPhotoService photoService, ILogger<ProductService> logger) : IProductService
{
    private readonly IMapper _mapper = mapper;
    private readonly IPhotoService _photoService = photoService;
    private readonly ILogger<ProductService> _logger = logger;
    private readonly DataContext _db = db;

    public async Task<Result<List<ProductDto>>> GetAllAsync(ProductQuerySpecs specs)
    {
        var query = _db.Products.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductEntity>(
            new SearchEvaluatorStrategy<ProductEntity>(specs.SearchTerm, new ProductSearchProvider()),
            new FilterEvaluatorStrategy<ProductEntity, bool>(new ProductFilterProvider().BuildFilter(specs)),
            new SortEvaluatorStrategy<ProductEntity>(specs.OrderBy, new ProductSortProvider()), 
            new SelectEvaluatorStrategy<ProductEntity>(specs.PageNumber, specs.PageSize)
        );

        var filtered = queryContext.ApplyQuery(query);
        var projected = await filtered.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        if (projected is null) return Result<List<ProductDto>>.Fail("Products not found...", ResultTypeEnum.NotFound);
        return Result<List<ProductDto>>.Success(projected, ResultTypeEnum.Success, query.Count());
    }

    public async Task<ProductDto?> GetAsync(int id)
    {
        var product = await _db.Products
                               .AsNoTracking()
                               .Where(p => p.Id == id)
                               .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                               .SingleOrDefaultAsync();
        return product;
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto Dto)
    {
        var model = _mapper.Map<ProductEntity>(Dto);
        PhotoDto? newPhoto = null;
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (Dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(new(Dto.Photo));
                model.SetPhoto(newPhoto.Id);
            }

            _db.Products.Add(model);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            var output = _mapper.Map<ProductDto>(model);
            return output;
        }
        catch
        {
            await transaction.RollbackAsync();
            if (newPhoto is not null) await _photoService.TryDeleteCloudAsync(newPhoto.PublicId);
            throw;
        }
    }

    public async Task<ProductDto> UpdateAsync(ProductUpdateDto dto)
    {
        var product = await _db.Products.Include(p => p.Photo).FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (product is null) throw new KeyNotFoundException($"Product {dto.Id} not found.");
        var oldPhoto = _mapper.Map<PhotoDto>(product.Photo);
        PhotoDto? newPhoto = null;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Save new photo.
            if (dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(new(dto.Photo));
                product.SetPhoto(newPhoto.Id);
            }

            _mapper.Map(dto, product);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Delete old photo
            if (oldPhoto is not null && newPhoto is not null && oldPhoto.Id != newPhoto.Id)
            {
                try
                {
                    await _photoService.DeleteAsync(oldPhoto);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old photo {PhotoId}", oldPhoto.Id);
                }
            }

            var output = _mapper.Map<ProductDto>(product);
            return output;
        }
        catch
        {
            await transaction.RollbackAsync();
            if (newPhoto is not null) await _photoService.TryDeleteCloudAsync(newPhoto.PublicId);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.Include(p => p.Photo).FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return false;
        var photo = _mapper.Map<PhotoDto>(product.Photo);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Delete product
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            await _db.Photos.Where(x => x.Id == photo.Id).ExecuteDeleteAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        if (!string.IsNullOrWhiteSpace(photo.PublicId))
        {
            try 
            { 
                await _photoService.TryDeleteCloudAsync(photo.PublicId); 
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete cloud photo {PublicId}", photo.PublicId);
            }
        }
        return true;
    }
}
