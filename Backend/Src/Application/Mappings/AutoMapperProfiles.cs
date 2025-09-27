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
        var productMap = CreateMap<ProductUpdateDto, ProductEntity>();
        productMap.ForAllMembers(x => x.Ignore());
        productMap.AfterMap((src, dest) =>
        {
            dest.SetName(src.Name);
            dest.SetDescription(src.Description);
            dest.SetBrandId(src.BrandId);
            dest.SetProductTypeId(src.ProductTypeId);
            dest.ChangeUnitPrice(src.UnitPrice);
            dest.SetStock(src.QuantityInStock);
        }); 

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
        CreateMap<BasketItemEntity, BasketItemDto>().ForMember(d => d.LineTotal, opt => opt.MapFrom(s => s.LineTotal));
        CreateMap<BasketItemAddRequest, BasketItemCreateDto>();
        CreateMap<BasketItemRemoveRequest, BasketItemDto>().ConvertUsing(src => new BasketItemDto
        {
            BasketId = src.BasketId,
            ProductId = src.ProductId,
            Quantity = src.Quantity
        });
        CreateMap<BasketItemDto, BasketItemResponse>().ConvertUsing(src => new BasketItemResponse
        {
            BasketId = src.BasketId,
            ProductId = src.ProductId,
            Quantity = src.Quantity,
            Name = src.Product!.Name,
            UnitPrice = src.UnitPrice,
            PublicUrl = src.Product!.Photo!.PublicUrl,
            LineTotal = src.LineTotal
        }); 

        // Basket
        CreateMap<BasketEntity, BasketDto>().ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Subtotal));
        CreateMap<BasketCouponRequest, BasketCouponDto>();
        CreateMap<BasketDto, BasketResponse>();

        // Order
        CreateMap<OrderEntity, OrderDto>().ForMember(d => d.Total, opt => opt.MapFrom(s => s.Total));
        CreateMap<OrderDto, OrderResponse>();
        CreateMap<CreateOrderRequest, OrderCreateDto>();

        // Order Item
        CreateMap<OrderItemEntity, OrderItemDto>().ForMember(d => d.PictureUrl, opt => opt.MapFrom(src => src.Product!.Photo!.PublicUrl));
        CreateMap<OrderItemDto, OrderItemResponse>();

        // Coupon
        CreateMap<CouponEntity, CouponDto>();
        CreateMap<CouponDto, CouponResponse>();
        CreateMap<CouponDto, CouponEntity>().ConstructUsing(s => new CouponEntity(s.Name, s.RemoteId, s.PromotionCode, s.AmountOff, s.PercentOff)).ForAllMembers(o => o.Ignore());
        CreateMap<CouponInfoModel, CouponDto>()
            .ForMember(d => d.AmountOff, opt => opt.ConvertUsing(new MinorUnitsToDecimalConverter(), src => src.AmountOff))
            .ForMember(d => d.Id, opt => opt.Ignore());

        // Address
        CreateMap<CreateAddressRequest, AddressCreateDto>();
        CreateMap<UpdateAddressRequest, AddressUpdateDto>();
        CreateMap<AddressDto, AddressResponse>().ReverseMap();
        CreateMap<AddressDto, AddressEntity>().ConstructUsing(s => new AddressEntity(s.Line1, s.Line2, s.City, s.State, s.PostalCode, s.Country)).ForAllMembers(o => o.Ignore());
        CreateMap<AddressCreateDto, AddressEntity>().ConstructUsing(s => new AddressEntity(s.Line1, s.Line2, s.City, s.State, s.PostalCode, s.Country)).ForAllMembers(o => o.Ignore());
        var addressMap = CreateMap<AddressUpdateDto, AddressEntity>();
        addressMap.ForAllMembers(x => x.Ignore());
        addressMap.AfterMap((src, dest) => dest.Update(src.Line1, src.Line2, src.City, src.State, src.PostalCode, src.Country));
        CreateMap<AddressEntity, AddressDto>();

        // Shipping address
        CreateMap<ShippingAddress, ShippingAddressDto>();
        CreateMap<CreateShippingAddressRequest, ShippingAddressDto>();
        CreateMap<ShippingAddressDto, ShippingAddressResponse>();
        CreateMap<ShippingAddressDto, ShippingAddress>().ConstructUsing(s => new ShippingAddress(s.RecipientName, s.Line1, s.Line2, s.City, s.State, s.PostalCode, s.Country)).ForAllMembers(o => o.Ignore());

        // Payment Summary
        CreateMap<PaymentSummary, PaymentSummaryDto>().ReverseMap();
        CreateMap<PaymentSummaryDto, PaymentSummaryResponse>().ReverseMap();
        CreateMap<CreatePaymentSummaryRequest, PaymentSummaryDto>();

        // User
        CreateMap<UserEntity, UserDto>()
            .ForMember(x => x.IsAuthenticated, opt => opt.Ignore())
            .ForMember(x => x.Roles, opt => opt.Ignore())
            .ForMember(x => x.Address, opt => opt.Ignore())
            .ForMember(x => x.AddressId, opt => opt.Ignore())
            .ForMember(x => x.IsActive, opt => opt.Ignore());
        CreateMap<UserRegisterRequest, UserRegisterDto>();
        CreateMap<UserDto, UserResponse>();
    }
}
