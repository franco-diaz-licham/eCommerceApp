namespace Backend.Configuration;

public static class MappingServices
{
    public static void AddMappingService(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationProvider>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var config = new MapperConfiguration(c => { c.AddProfile<AutoMapperProfiles>(); }, loggerFactory);
            config.AssertConfigurationIsValid();
            return config;
        });

        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>()));
    }
}
