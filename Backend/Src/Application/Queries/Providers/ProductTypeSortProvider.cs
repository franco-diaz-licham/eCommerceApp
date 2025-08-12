namespace Backend.Src.Application.Queries.Providers;

public class ProductTypeSortProvider : ISortEvaluatorProvider<ProductTypeEntity>
{
    private readonly Dictionary<string, SortExpression<ProductTypeEntity>> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        { "nameAsc", new (p => p.Name, false) },
        { "nameDesc", new (p => p.Name, true)  },
        { "createdAsc", new (p => p.CreatedOn, false) },
        { "createdDesc",new (p => p.CreatedOn, true)  }
    };

    public bool GetSorter(string? orderBy, out SortExpression<ProductTypeEntity>? sort) => _map.TryGetValue(orderBy ?? "", out sort);
}
