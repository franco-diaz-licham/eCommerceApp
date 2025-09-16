namespace Backend.Configuration;

public static class AppServices
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        // App services
        builder.Services.AddScoped<IMediaStorageService, CloudinaryService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IFilterService, FilterService>();
        builder.Services.AddScoped<IBrandService, BrandService>();
        builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
        builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
        builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        builder.Services.AddScoped<IBasketService, BasketService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

        // Repos
        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }
}
