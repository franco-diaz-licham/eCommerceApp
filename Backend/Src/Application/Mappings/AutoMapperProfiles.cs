namespace Backend.Src.Application.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Product
        CreateMap<ProductDto, ProductResponse>();
        CreateMap<ProductEntity, ProductDto>();
        CreateMap<ProductQueryParams, ProductQuerySpecs>();
        CreateMap<ProductCreateDto, ProductEntity>().ConstructUsing(s => new ProductEntity(s.Name, s.Description, s.UnitPrice, s.QuantityInStock, s.ProductTypeId, s.BrandId)).ForAllMembers(o => o.Ignore());
        var map = CreateMap<ProductUpdateDto, ProductEntity>();
        map.ForAllMembers(opt => opt.Ignore());
        map.AfterMap<ProductUpdateAction>();
        CreateMap<UpdateProductRequest, ProductUpdateDto>();
        CreateMap<CreateProductRequest, ProductCreateDto>();

        // Photo
        CreateMap<PhotoEntity, PhotoDto>();
        CreateMap<PhotoDto, PhotoResponse>();
        CreateMap<PhotoDto, PhotoEntity>().ConstructUsing(s => new PhotoEntity(s.FileName, s.PublicId, s.PublicUrl)).ForAllMembers(o => o.Ignore());

        // Brand
        CreateMap<BrandEntity, BrandDto>();
        CreateMap<BrandDto, BrandEntity>().ConstructUsing(s => new BrandEntity(s.Name)).ForAllMembers(o => o.Ignore());
        CreateMap<BrandDto, BrandResponse>();

        // ProductType
        CreateMap<ProductTypeEntity, ProductTypeDto>();
        CreateMap<ProductTypeDto, ProductTypeResponse>();
        CreateMap<ProductTypeDto, ProductTypeEntity>().ConstructUsing(s => new ProductTypeEntity(s.Name)).ForAllMembers(o => o.Ignore());

        // OrderStatus
        CreateMap<OrderStatusEntity, OrderStatusDto>();
        CreateMap<OrderStatusDto, OrderStatusResponse>();
        CreateMap<OrderStatusDto, OrderStatusEntity>().ConstructUsing(s => new OrderStatusEntity(s.Name)).ForAllMembers(o => o.Ignore());

        // Query
        CreateMap<BaseQueryParams, BaseQuerySpecs>();

        // Open-generic PagedList mapping (Dto -> Response)
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        CreateMap(typeof(Result<>), typeof(Result<>)).ConvertUsing(typeof(ResultConverter<,>));

        // Basket items
        CreateMap<BasketItemEntity, BasketItemCreateDto>();
        CreateMap<BasketItemEntity, BasketItemDto>().ForMember(d => d.LineTotal, opt => opt.MapFrom(s => s.LineTotal));

        // Basket
        CreateMap<BasketEntity, BasketDto>().ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Subtotal));
        CreateMap<BasketCouponRequest, BasketCouponDto>();

        // Order
        CreateMap<OrderEntity, OrderDto>().ForMember(d => d.Total, opt => opt.MapFrom(s => s.Total));
        CreateMap<OrderDto, OrderResponse>();
        CreateMap<CreateOrderRequest, OrderCreateDto>().ForMember(x => x.UserEmail, o => o.Ignore());

        // Order Item
        CreateMap<OrderItemEntity, OrderItemDto>();
        CreateMap<OrderItemDto, OrderItemResponse>();

        // Coupon
        CreateMap<CouponEntity, CouponDto>();
        CreateMap<CouponDto, CouponEntity>().ConstructUsing(s => new CouponEntity(s.Name, s.RemoteId, s.PromotionCode, s.AmountOff, s.PercentOff)).ForAllMembers(o => o.Ignore());
        CreateMap<CouponInfoModel, CouponDto>()
            .ForMember(d => d.AmountOff, opt => opt.ConvertUsing(new MinorUnitsToDecimalConverter(), src => src.AmountOff))
            .ForMember(d => d.Id, opt => opt.Ignore());

        // Address
        CreateMap<ShippingAddress, AddressDto>();
        CreateMap<AddressDto, ShippingAddress>();
        CreateMap<AddressDto, AddressResponse>().ReverseMap();
        CreateMap<AddressDto, AddressEntity>().ConstructUsing(s => new AddressEntity(s.Line1, s.Line2, s.City, s.State, s.PostalCode, s.Country)).ForAllMembers(o => o.Ignore());

        // Payment Summary
        CreateMap<PaymentSummary, PaymentSummaryDto>().ReverseMap();
        CreateMap<PaymentSummaryDto, PaymentSummaryResponse>().ReverseMap();
    }
}
