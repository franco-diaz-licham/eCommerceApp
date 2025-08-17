namespace Backend.Src.Domain.Entities;

public class OrderItemEntity : BaseEntity
{
    private OrderItemEntity() { }
    public OrderItemEntity(int productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId <= 0) throw new ArgumentException("Invalid product.");
        if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Product name required.");
        if (unitPrice < 0m) throw new ArgumentException("Unit price cannot be negative.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        ProductName = productName;
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
    public decimal LineTotal => UnitPrice * Quantity;
    #endregion

    #region Business logic
    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        checked { Quantity += quantity; }
        Touch();
    }

    internal void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (quantity > Quantity) throw new ArgumentException("Cannot reduce below zero.");
        Quantity -= quantity;
        Touch();
    }

    internal void SetUnitPrice(decimal price)
    {
        if (price < 0m) throw new ArgumentException("Unit price cannot be negative.");
        UnitPrice = price;
        Touch();
    }
    
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
