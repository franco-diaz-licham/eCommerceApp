namespace Backend.Src.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    private readonly ILogger<ProductService> _logger;
    private readonly DataContext _db;

    public ProductService(DataContext db, IMapper mapper, IPhotoService photoService, ILogger<ProductService> logger)
    {
        _db = db;
        _mapper = mapper;
        _photoService = photoService;
        _logger = logger;
    }

    /// <summary>
    /// Method which gets all products.
    /// </summary>
    public IQueryable<ProductDTO> GetAllAsync(ProductQuerySpecs specs)
    {
        var baseQuery = _db.Products.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductEntity>(
            new SearchEvaluatorStrategy<ProductEntity>(specs.SearchTerm, new ProductSearchProvider()),
            new FilterEvaluatorStrategy<ProductEntity, bool>(new ProductFilterProvider().BuildFilter(specs)),
            new SortEvaluatorStrategy<ProductEntity>(specs.OrderBy, new ProductSortProvider())
        );

        var query = queryContext.Execute(baseQuery);
        var output = query.ProjectTo<ProductDTO>(_mapper.ConfigurationProvider);
        return output;
    }

    /// <summary>
    /// Method which gets a Product.
    /// </summary>
    public async Task<ProductDTO?> GetAsync(int id)
    {
        var product = await _db.Products
                               .AsNoTracking()
                               .Where(p => p.Id == id)
                               .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
                               .SingleOrDefaultAsync();
        return product;
    }

    /// <summary>
    /// Method creates a product.
    /// </summary>
    public async Task<ProductDTO> CreateAsync(ProductCreateDTO dto)
    {
        var model = _mapper.Map<ProductEntity>(dto);
        PhotoDTO? newPhoto = null;
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(dto.Photo);
                model.SetPhoto(newPhoto.Id);
            }

            _db.Products.Add(model);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            var output = _mapper.Map<ProductDTO>(model);
            return output;
        }
        catch
        {
            await transaction.RollbackAsync();
            if (newPhoto is not null) await _photoService.TryDeleteCloudAsync(newPhoto.PublicId);
            throw;
        }
    }

    /// <summary>
    /// Method which updates a product.
    /// </summary>
    public async Task<ProductDTO> UpdateAsync(ProductUpdateDTO dto)
    {
        var product = await _db.Products.Include(p => p.Photo).FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (product is null) throw new KeyNotFoundException($"Product {dto.Id} not found.");
        var oldPhoto = _mapper.Map<PhotoDTO>(product.Photo);
        PhotoDTO? newPhoto = null;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Save new photo.
            if (dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(dto.Photo);
                product.SetPhoto(newPhoto.Id);
            }

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

            var output = _mapper.Map<ProductDTO>(product);
            return output;
        }
        catch
        {
            await transaction.RollbackAsync();
            if (newPhoto is not null) await _photoService.TryDeleteCloudAsync(newPhoto.PublicId);
            throw;
        }
    }

    /// <summary>
    /// Method which deletes a product.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.Include(p => p.Photo).FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return false;
        var photo = _mapper.Map<PhotoDTO>(product.Photo);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Delete product
            _db.Products.Remove(product);
            await _db.Photos.Where(x => x.Id == photo.Id).ExecuteDeleteAsync();
            await _db.SaveChangesAsync();
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
