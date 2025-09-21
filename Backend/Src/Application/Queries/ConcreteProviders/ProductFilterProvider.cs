namespace Backend.Src.Application.Queries.Providers;

public sealed class ProductFilterProvider : IFilterExpressionProvider<ProductEntity>
{
    public IEnumerable<Expression<Func<ProductEntity, bool>>> BuildFilter(ProductQuerySpecs specs)
    {
        if (specs.BrandIds != null && specs.BrandIds.Count > 0)
        {
            var ids = specs.BrandIds.ToArray();
            yield return p => ids.Contains(p.BrandId);
        }

        if (specs.ProductTypeIds != null && specs.ProductTypeIds.Count > 0)
        {
            var ids = specs.ProductTypeIds.ToArray();
            yield return p => ids.Contains(p.ProductTypeId);
        }
    }
}
