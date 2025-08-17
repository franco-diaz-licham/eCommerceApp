namespace Backend.Src.Application.Mappings;

public sealed class ProductUpdateAction : IMappingAction<ProductUpdateDTO, ProductEntity>
{
    public void Process(ProductUpdateDTO s, ProductEntity d, ResolutionContext context)
    {
        d.SetName(s.Name);
        d.SetDescription(s.Description);
        d.ChangeUnitPrice(s.UnitPrice);
        d.IncreaseStock(s.QuantityInStock);
    }
}