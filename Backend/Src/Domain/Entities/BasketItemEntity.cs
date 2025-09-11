namespace Backend.Src.Domain.Entities;

public class BasketItemEntity
{
    private BasketItemEntity() { }
    public BasketItemEntity(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        if (productId <= 0) throw new ArgumentException($"{nameof(productId)} cannot be less than 0.");
        if (unitPrice < 0m) throw new ArgumentException($"{nameof(unitPrice)} cannot be negative.");
        if (quantity <= 0) throw new ArgumentException($"{nameof(quantity)} must be positive.");

        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Product = product; 
    }

    #region Properties
    public int BasketId { get; private set; }
    public BasketEntity? Basket { get; private set; }
    public int ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;
    #endregion

    #region Business logic
    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException($"{nameof(quantity)} must be positive.");
        checked { Quantity += quantity; }
    }

    public void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException($"{nameof(quantity)} must be positive.");
        if (quantity > Quantity) throw new ArgumentOutOfRangeException($"Cannot reduce {nameof(quantity)} below zero.");
        Quantity -= quantity;
    }

    public void ReplaceQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException($"{nameof(quantity)} must be positive.");
        Quantity = quantity;
    }

    public void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException($"{nameof(unitPrice)} cannot be negative.");
        UnitPrice = unitPrice;
    }
    #endregion
}
