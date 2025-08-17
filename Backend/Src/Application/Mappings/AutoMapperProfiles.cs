namespace Backend.Src.Application.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Product
        CreateMap<ProductDTO, ProductResponse>();
        CreateMap<ProductEntity, ProductDTO>();
        CreateMap<ProductQueryParams, ProductQuerySpecs>()
            .ForMember(d => d.PageSize, o => o.Ignore());
        CreateMap<ProductCreateDTO, ProductEntity>()
            .ConstructUsing(s => new ProductEntity(s.Name, s.Description, s.UnitPrice, s.QuantityInStock, s.ProductTypeId, s.BrandId, null))
            .ForAllMembers(o => o.Ignore());
        var map = CreateMap<ProductUpdateDTO, ProductEntity>();
        map.ForAllMembers(opt => opt.Ignore());
        map.AfterMap< ProductUpdateAction>();

        // Photo
        CreateMap<PhotoEntity, PhotoDTO>();
        CreateMap<PhotoDTO, PhotoResponse>();
        CreateMap<PhotoCreateRequest, PhotoCreateDTO>();
        CreateMap<PhotoDTO, PhotoEntity>()
            .ConstructUsing(s => new PhotoEntity(s.FileName, s.PublicId, s.PublicUrl))
            .ForAllMembers(o => o.Ignore());

        // Brand
        CreateMap<BrandEntity, BrandDTO>();
        CreateMap<BrandDTO, BrandEntity>()
            .ConstructUsing(s => new BrandEntity(s.Name))
            .ForAllMembers(o => o.Ignore());
        CreateMap<BrandDTO, BrandResponse>();

        // ProductType
        CreateMap<ProductTypeEntity, ProductTypeDTO>();
        CreateMap<ProductTypeDTO, ProductTypeResponse>();
        CreateMap<ProductTypeDTO, ProductTypeEntity>()
            .ConstructUsing(s => new ProductTypeEntity(s.Name))
            .ForAllMembers(o => o.Ignore());

        // OrderStatus
        CreateMap<OrderStatusEntity, OrderStatusDTO>();
        CreateMap<OrderStatusDTO, OrderStatusResponse>();
        CreateMap<OrderStatusDTO, OrderStatusEntity>()
            .ConstructUsing(s => new OrderStatusEntity(s.Name))
            .ForAllMembers(o => o.Ignore());

        // Query
        CreateMap<BaseQueryParams, BaseQuerySpecs>()
            .ForMember(d => d.PageSize, o => o.Ignore());

        // Open-generic PagedList mapping (DTO -> Response)
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        CreateMap(typeof(Result<>), typeof(Result<>)).ConvertUsing(typeof(ResultConverter<,>));

        // Basket items
        CreateMap<BasketItemEntity, BasketItemCreateDTO>();
        CreateMap<BasketItemEntity, BasketItemDTO>()
                .ForMember(d => d.LineTotal, opt => opt.MapFrom(s => s.LineTotal));

        // Basket
        CreateMap<BasketEntity, BasketDTO>()
                .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Subtotal));
        CreateMap<BasketCouponRequest, BasketCouponDTO>();

        // Order
        CreateMap<OrderEntity, OrderDTO>().ForMember(d => d.Total, opt => opt.MapFrom(s => s.Total));

        // Order Item
        CreateMap<OrderItemEntity, OrderItemDTO>();

        // Coupon
        CreateMap<CouponEntity, CouponDTO>();

        // Address
        CreateMap<ShippingAddress, AddressDTO>();
        CreateMap<AddressDTO, ShippingAddress>();
        CreateMap<AddressDTO, AddressEntity>()
            .ConstructUsing(s => new AddressEntity(s.Line1, s.Line2, s.City, s.State, s.PostalCode, s.Country))
            .ForAllMembers(o => o.Ignore());

        // Payment Summary
        CreateMap<PaymentSummary, PaymentSummaryDTO>();
    }
}
