namespace Backend.Src.Application.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Product
        CreateMap<ProductDTO, ProductResponse>();
        CreateMap<ProductEntity, ProductDTO>().ReverseMap();
        CreateMap<ProductCreateDTO, ProductEntity>()
            .ForMember(d => d.Photo, opt => opt.Ignore())
            .ForMember(d => d.PhotoId, opt => opt.Ignore());
        CreateMap<ProductUpdateDTO, ProductEntity>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Photo, opt => opt.Ignore())
            .ForMember(d => d.PhotoId, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ProductQueryParams, ProductQuerySpecs>();
                
        // Photo
        CreateMap<PhotoEntity, PhotoDTO>().ReverseMap();
        CreateMap<PhotoDTO, PhotoResponse>();
        CreateMap<PhotoCreateRequest, PhotoDTO>();

        // Brand
        CreateMap<BrandDTO, BrandEntity>().ReverseMap();
        CreateMap<BrandDTO, BrandResponse>();

        // ProductType
        CreateMap<ProductTypeDTO, ProductTypeEntity>().ReverseMap();
        CreateMap<ProductTypeDTO, ProductTypeResponse>();

        // OrderStatus
        CreateMap<OrderStatusDTO, OrderStatusEntity>().ReverseMap();
        CreateMap<OrderStatusDTO, OrderStatusResponse>();

        // Query
        CreateMap<BaseQueryParams, BaseQuerySpecs>();

        // Open-generic PagedList mapping (DTO -> Response)
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
    }

    /// <summary>
    /// Create a generic pagelist mapper.
    /// </summary>
    public sealed class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination>? destination, ResolutionContext context)
        {
            var mappedItems = source.Select(x => context.Mapper.Map<TDestination>(x)).ToList();
            return new PagedList<TDestination>(mappedItems, source.Metadata.TotalCount, source.Metadata.PageNumber, source.Metadata.PageSize);
        }
    }
}
