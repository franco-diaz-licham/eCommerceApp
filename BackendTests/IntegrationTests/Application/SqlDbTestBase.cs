namespace BackendTests.IntegrationTests.Application;

public class SqlDbTestBase : IAsyncLifetime
{
    private const string SQL = "Filename=:memory:";
    protected SqliteConnection Conn = default!;
    protected DbContextOptions<DataContext> Options = default!;
    protected IMapper Mapper = default!;

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

    protected DataContext Context() => new DataContext(Options);
}
