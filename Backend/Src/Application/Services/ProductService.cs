using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend.Src.Application.Services;

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
    public async Task<PagedList<ProductDto>> GetAllAsync(ProductQuerySpecs specs)
    {
        var query = _db.Products.AsNoTracking();
        var queryContext = new QueryStrategyContext<ProductEntity>(
            new SearchEvaluatorStrategy<ProductEntity>(specs.SearchTerm, new ProductSearchProvider()),
            new FilterEvaluatorStrategy<ProductEntity, bool>(new ProductFilterProvider().BuildFilter(specs)),
            new SortEvaluatorStrategy<ProductEntity>(specs.OrderBy, new ProductSortProvider())
        );

        var filtered = queryContext.ApplyQuery(query);
        var count = await filtered.CountAsync();
        var projected = filtered.ProjectTo<ProductDto>(_mapper.ConfigurationProvider);
        return await PagedList<ProductDto>.ToPagedList(projected, count, specs.PageNumber, specs.PageSize);
    }

    /// <summary>
    /// Method which gets a Product.
    /// </summary>
    public async Task<ProductDto?> GetAsync(int id)
    {
        var product = await _db.Products
                               .AsNoTracking()
                               .Where(p => p.Id == id)
                               .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                               .SingleOrDefaultAsync();
        return product;
    }

    /// <summary>
    /// Method creates a product.
    /// </summary>
    public async Task<ProductDto> CreateAsync(ProductCreateDto Dto)
    {
        var model = _mapper.Map<ProductEntity>(Dto);
        PhotoDto? newPhoto = null;
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (Dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(Dto.Photo);
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

    /// <summary>
    /// Method which updates a product.
    /// </summary>
    public async Task<ProductDto> UpdateAsync(ProductUpdateDto Dto)
    {
        var product = await _db.Products.Include(p => p.Photo).FirstOrDefaultAsync(p => p.Id == Dto.Id);
        if (product is null) throw new KeyNotFoundException($"Product {Dto.Id} not found.");
        var oldPhoto = _mapper.Map<PhotoDto>(product.Photo);
        PhotoDto? newPhoto = null;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Save new photo.
            if (Dto.Photo is not null)
            {
                newPhoto = await _photoService.CreateImageAsync(Dto.Photo);
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

    /// <summary>
    /// Method which deletes a product.
    /// </summary>
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
