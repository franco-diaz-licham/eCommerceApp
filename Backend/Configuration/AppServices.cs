namespace Backend.Configuration;

public static class AppServices
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IImageStorageService, CloudinaryService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IPaginationService, PaginationService>();
        builder.Services.AddScoped<IFilterService, FilterService>();
        builder.Services.AddScoped<IBrandService, BrandService>();
        builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
        builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
        builder.Services.AddScoped<IRemotePaymentService, StripeService>();
        builder.Services.AddScoped<IBasketService, BasketService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
    }
}
