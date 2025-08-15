namespace Backend.Src.Domain.Entities;

public class OrderItemEntity
{
    private OrderItemEntity() { }
    public OrderItemEntity(int productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId <= 0) throw new ArgumentException("Invalid product.");
        if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Product name required.");
        if (unitPrice < 0m) throw new ArgumentException("Unit price cannot be negative.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

        ProductId = productId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
        CreatedOn = DateTime.UtcNow;
    }

    #region Properties
    public int OrderId { get; private set; }
    public OrderEntity Order { get; private set; } = default!;
    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public ProductEntity? Product { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    [NotMapped] public decimal LineTotal => UnitPrice * Quantity;
    #endregion

    #region Business logic
    internal void IncreaseQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
        checked { Quantity += qty; }
        Touch();
    }

    internal void DecreaseQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
        if (qty > Quantity) throw new ArgumentException("Cannot reduce below zero.");
        Quantity -= qty;
        Touch();
    }

    internal void SetUnitPrice(decimal newPrice)
    {
        if (newPrice < 0m) throw new ArgumentException("Unit price cannot be negative.");
        UnitPrice = newPrice;
        Touch();
    }
    
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
