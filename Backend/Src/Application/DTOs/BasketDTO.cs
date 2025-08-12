namespace Backend.Src.Application.DTOs;

public class BasketDTO
{
    public int Id { get; set; }
    public required string BasketId { get; set; }
    public List<BasketItemDTO> Items { get; set; } = [];
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
    public AppCouponDTO? Coupon { get; set; }

    public void AddItem(ProductEntity product, int quantity)
    {
        if (product == null) ArgumentNullException.ThrowIfNull(product);
        if (quantity <= 0) throw new ArgumentException("Quantity should be greater than zero", nameof(quantity));
        var existingItem = FindItem(product.Id);
        if (existingItem is null)
        {
            Items.Add(new BasketItemDTO
            {
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            existingItem.Quantity += quantity;
        }
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity should be greater than zero", nameof(quantity));
        var item = FindItem(productId);
        if (item is null) return;
        item.Quantity -= quantity;
        if (item.Quantity <= 0) Items.Remove(item);
    }

    private BasketItemDTO? FindItem(int productId)
    {
        return Items.FirstOrDefault(item => item.ProductId == productId);
    }
}
