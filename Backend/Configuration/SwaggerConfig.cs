namespace Backend.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerceApp API v1" });
        });
        return services;
    }
}
