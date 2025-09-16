namespace Backend.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerceApp API v1" });
            opt.DocumentFilter<HideIdentityRegisterFilter>();
        });
        return services;
    }
}

/// <summary>
/// Remove local /register endpoint for custom one.
/// </summary>
public sealed class HideIdentityRegisterFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var removeKeys = swaggerDoc.Paths.Keys
            .Where(k => k.EndsWith("/api/Account/register", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in removeKeys) swaggerDoc.Paths.Remove(key);
    }
}
