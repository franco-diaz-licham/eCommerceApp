namespace Backend.Src.Domain.Entities;

public class BasketItemEntity
{
    private BasketItemEntity() { }
    public BasketItemEntity(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));

        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Product = product; 
    }

    #region Properties
    public int BasketId { get; set; }
    public BasketEntity? Basket { get; set; }
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    [NotMapped] public decimal LineTotal => UnitPrice * Quantity;
    #endregion

    #region Business logic
    public void IncreaseQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        checked { Quantity += qty; }
    }

    public void DecreaseQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        if (qty > Quantity) throw new ArgumentOutOfRangeException("Cannot reduce below zero.");
        Quantity -= qty;
    }

    public void ReplaceQuantity(int newQty)
    {
        if (newQty <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        Quantity = newQty;
    }

    public void SetUnitPrice(decimal newPrice)
    {
        if (newPrice < 0m) throw new ArgumentOutOfRangeException("Unit price cannot be negative.");
        UnitPrice = newPrice;
    }
    #endregion
}
