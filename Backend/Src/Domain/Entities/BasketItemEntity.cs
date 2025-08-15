namespace Backend.Src.Domain.Entities;

public class BasketItemEntity : BaseEntity
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

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public void Increase(int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        Quantity += qty;
    }

    public void Decrease(int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        Quantity = Math.Max(0, Quantity - qty);
    }

    public decimal LineTotal() => UnitPrice * Quantity;
}
