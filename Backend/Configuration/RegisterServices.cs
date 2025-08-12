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
            opt.UseNpgsql(builder.Configuration.GetConnectionString("StoreDb") ?? "").UseSnakeCaseNamingConvention();
            opt.LogTo(Console.WriteLine, LogLevel.Information);
        });
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
        builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        builder.Services.AddMemoryCache();
        builder.AddAppServices();
        builder.AddIdentityServices();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // model validation API response.
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
