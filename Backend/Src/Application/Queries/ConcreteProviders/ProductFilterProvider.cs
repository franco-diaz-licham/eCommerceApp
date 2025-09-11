namespace Backend.Src.Application.Queries.Providers;

public sealed class ProductFilterProvider: IFilterExpressionProvider<ProductEntity, ProductQuerySpecs>
{
    public IEnumerable<Expression<Func<ProductEntity, bool>>> BuildFilter(ProductQuerySpecs s)
    {
        if (s.BrandIds != null && s.BrandIds.Count > 0)
        {
            var ids = s.BrandIds.ToArray();
            yield return p => ids.Contains(p.BrandId);
        }

        if (s.ProductTypeIds != null && s.ProductTypeIds.Count > 0)
        {
            var ids = s.ProductTypeIds.ToArray();
            yield return p => ids.Contains(p.ProductTypeId);
        }
    }
}
