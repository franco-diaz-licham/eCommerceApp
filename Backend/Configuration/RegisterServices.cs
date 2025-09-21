using Backend.Src.Infrastructure.Persistence;

namespace Backend.Configuration;

public static class RegisterServices
{
    /// <summary>
    /// Method which registers all services used in the application to the DI container.
    /// </summary>
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerService();
        builder.Services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(builder.Configuration.GetConnectionString("StoreDb") ?? "");
            opt.LogTo(Console.WriteLine, LogLevel.Information);
        });
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
        builder.Services.AddMappingService();
        builder.Services.AddMemoryCache();
        builder.AddAppServices();
        builder.AddIdentityServices();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins("https://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Model validation API response.
        builder.Services.Configure<ApiBehaviorOptions>(opt =>
        {
            opt.InvalidModelStateResponseFactory = actionContext =>
            {
                // make all validations errors into array
                var errors = actionContext.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .SelectMany(x => x.Value?.Errors!)
                            .Select(x => x.ErrorMessage).ToArray();

                var errorResponse = new ApiValidationErrorResponse(errors);
                return new BadRequestObjectResult(errorResponse);
            };
        });
    }
}
