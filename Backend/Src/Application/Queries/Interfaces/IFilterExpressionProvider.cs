namespace Backend.Src.Application.Queries.Interfaces;

public interface IFilterExpressionProvider<T>
{
    IEnumerable<Expression<Func<T, bool>>> BuildFilter(ProductQuerySpecs specs);
}
