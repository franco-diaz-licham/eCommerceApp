namespace Backend.Src.Application.Mappings;

/// <summary>
/// Create a generic pagelist mapper.
/// </summary>
public sealed class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
{
    public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination>? destination, ResolutionContext context)
    {
        // Map every individual item
        var mappedItems = source.Select(x => context.Mapper.Map<TDestination>(x)).ToList();
        return new PagedList<TDestination>(mappedItems, source.Metadata.TotalCount, source.Metadata.PageNumber, source.Metadata.PageSize);
    }
}

/// <summary>
/// Converts a generi result mapper.
/// </summary>
public sealed class ResultConverter<TSource, TDestination> : ITypeConverter<Result<TSource>, Result<TDestination>>
{
    public Result<TDestination> Convert(Result<TSource> source, Result<TDestination>? destination, ResolutionContext context)
    {
        if (!source.IsSuccess) return Result<TDestination>.Fail(source.Error?.Message ?? "Problem...", source.Type);
        if (source.Value is null) return Result<TDestination>.Success(source.Type);
        if (typeof(TSource) == typeof(TDestination)) return Result<TDestination>.Success((TDestination)(object)source.Value!, source.Type, source.TotalCount);
        return Result<TDestination>.Success(context.Mapper.Map<TDestination>(source.Value), source.Type, source.TotalCount);
    }
}

public class MinorUnitsToDecimalConverter : IValueConverter<long?, decimal?>
{
    public decimal? Convert(long? sourceMember, ResolutionContext context) => sourceMember.HasValue ? sourceMember.Value / 100m : (decimal?)null;
}