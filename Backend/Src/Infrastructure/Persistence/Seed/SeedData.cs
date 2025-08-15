namespace Backend.Src.Infrastructure.Persistence.Seed;

public static class SeedData
{
    private const string BASE_PATH = "src/Infrastructure/Persistence/Seed";
    private const string PRODUCTS = $"{BASE_PATH}/ProductsData.json";
    private const string BRANDS = $"{BASE_PATH}/BrandsData.json";
    private const string PRODUCT_TYPES = $"{BASE_PATH}/ProductTypesData.json";
    private const string PHOTOS = $"{BASE_PATH}/PhotosData.json";
    private const string ORDER_STATUS = $"{BASE_PATH}/OrderStatusData.json";
    private const string IMAGES = $"{BASE_PATH}/Images/";
    public static async Task SeedAsync(DataContext db, IImageStorageService imageStoreService)
    {
        await Photos(db, imageStoreService);
        await ProductTypes(db);
        await OrderStatus(db);
        await Brands(db);
        await Products(db);
    }

    /// <summary>
    /// Migrates initial order status.
    /// </summary>
    private static async Task OrderStatus(DataContext db)
    {
        if (await db.OrderStatuses.AnyAsync()) return;
        var data = await File.ReadAllTextAsync(ORDER_STATUS);
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<List<OrderStatusEntity>>(data, opt);
        if (models is null) return;
        await db.OrderStatuses.AddRangeAsync(models);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Migrates intial brands.
    /// </summary>
    private static async Task Brands(DataContext db)
    {
        if (await db.Brands.AnyAsync()) return;
        var data = await File.ReadAllTextAsync(BRANDS);
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<List<BrandEntity>>(data, opt);
        if (models is null) return;
        await db.Brands.AddRangeAsync(models);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Migrates initial product types.
    /// </summary>
    private static async Task ProductTypes(DataContext db)
    {
        if (await db.ProductTypes.AnyAsync()) return;
        var data = await File.ReadAllTextAsync(PRODUCT_TYPES);
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<List<ProductTypeEntity>>(data, opt);
        if (models is null) return;
        await db.ProductTypes.AddRangeAsync(models);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Migrations initial photos.
    /// </summary>
    private static async Task Photos(DataContext db, IImageStorageService imageStoreService)
    {
        if (await db.Photos.AnyAsync()) return;
        // Load actors
        var data = await File.ReadAllTextAsync(PHOTOS);
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<List<PhotoEntity>>(data, opt);
        if (models is null) return;

        // Load images
        var imagesDirectory = Path.Combine(IMAGES);
        var imageFiles = Directory.GetFiles(imagesDirectory, "*.jpg");

        // Upload to cloudinary and update photo model.
        for (int i = 0; i < imageFiles.Length; i++)
        {
            var fileName = imageFiles[i].Substring(IMAGES.Length);
            byte[] bytes = await File.ReadAllBytesAsync(imageFiles[i]);
            var transform = new Transformation().Height(800).Width(500).Crop("fill");
            ImageUploadResult result = await imageStoreService.UploadPhotoAsync(bytes, fileName, transform);
            models[i].SetFileName(fileName);
            models[i].ReplaceRemote(result.PublicId.ToString(), result.SecureUrl.ToString());
        }

        await db.Photos.AddRangeAsync(models);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Migrates initial products.
    /// </summary>
    private static async Task Products(DataContext db)
    {
        if (await db.Products.AnyAsync()) return;
        var data = await File.ReadAllTextAsync(PRODUCTS);
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var models = JsonSerializer.Deserialize<List<ProductEntity>>(data, opt);
        if (models is null) return;
        await db.Products.AddRangeAsync(models);
        await db.SaveChangesAsync();
    }
}
