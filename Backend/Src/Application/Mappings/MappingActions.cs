namespace Backend.Src.Application.Mappings;

public sealed class ProductUpdateAction : IMappingAction<ProductUpdateDto, ProductEntity>
{
    public void Process(ProductUpdateDto s, ProductEntity d, ResolutionContext context)
    {
        d.SetName(s.Name);
        d.SetDescription(s.Description);
        d.ChangeUnitPrice(s.UnitPrice);
        d.IncreaseStock(s.QuantityInStock);
    }
}