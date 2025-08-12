namespace Backend.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerce Store API v1" });
        });
        return services;
    }
}
