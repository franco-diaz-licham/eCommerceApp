namespace Backend.Src.Application.Interfaces
{
    public interface IFilterExpressionProvider<T, TSpecs>
    {
        IEnumerable<Expression<Func<T, bool>>> BuildFilter(TSpecs specs);
    }
}
