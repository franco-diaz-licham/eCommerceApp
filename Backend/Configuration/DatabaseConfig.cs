namespace Backend.Configuration;

public static class DatabaseConfig
{
    /// <summary>
    /// Configures, migrates and seed any data to the database.
    /// </summary>
    public static async Task ConfigureDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var db = services.GetRequiredService<DataContext>() ?? throw new InvalidOperationException("No data context...");
            var imageStoreService = services.GetRequiredService<IMediaStorageService>();
            var userManager = services.GetRequiredService<UserManager<UserEntity>>() ?? throw new InvalidOperationException("No data context...");
            await db.Database.MigrateAsync();
            await SeedData.SeedAsync(db, userManager, imageStoreService);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration");
        }
    }
}
