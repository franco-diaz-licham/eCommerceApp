namespace BackendTests.IntegrationTests.Application;

public class SqlDbTestBase : IAsyncLifetime
{
    private const string SQL = "Filename=:memory:";
    protected SqliteConnection Conn = default!;
    protected DbContextOptions<DataContext> Options = default!;
    protected IMapper Mapper = default!;
    protected DataContext Context = default!;

    public async Task InitializeAsync()
    {
        SQLitePCL.Batteries.Init();
        Conn = new SqliteConnection(SQL);
        await Conn.OpenAsync();
        Options = new DbContextOptionsBuilder<DataContext>().UseSqlite(Conn).EnableSensitiveDataLogging().Options;

        // real schema
        using var ctx = new DataContext(Options);
        await ctx.Database.EnsureCreatedAsync();

        // Automapper
        var config = new MapperConfiguration(c =>
        {
            c.AddProfile<AutoMapperProfiles>();
        }, new LoggerFactory());

        config.AssertConfigurationIsValid();
        Mapper = config.CreateMapper();
    }

    public async Task DisposeAsync()
    {
        await Conn.DisposeAsync();
    }

    protected DataContext CreateContext() => Context = new DataContext(Options);
    protected Mock<DataContext> CreateMockContext() => new Mock<DataContext>(Options) { CallBase = true };

    protected async Task SeedAsync(params object[] entities)
    {
        foreach (var e in entities) Context.Add(e);
        await Context.SaveChangesAsync();
    }

    protected async Task<ProductEntity> CreateProductAsync()
    {
        var brand = new BrandEntity("Puma");
        var type = new ProductTypeEntity("Shoes");
        var photo = new PhotoEntity("photo1", "123456789", "https://test/com/photo1.jpg");

        await SeedAsync(brand);
        await SeedAsync(type);
        await SeedAsync(photo);

        var product = new ProductEntity("sku-1", "Prod 1", 10.00m, 5, brand.Id, type.Id, photoId: photo.Id, brand: brand, type: type);
        await SeedAsync(product);
        return product;
    }
}
