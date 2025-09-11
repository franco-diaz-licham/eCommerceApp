namespace Backend.Src.Application.Queries.Interfaces
{
    public interface IFilterExpressionProvider<T, TSpecs>
    {
        IEnumerable<Expression<Func<T, bool>>> BuildFilter(TSpecs specs);
    }
}
