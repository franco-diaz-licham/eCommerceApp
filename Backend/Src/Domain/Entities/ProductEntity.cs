namespace Backend.Src.Domain.Entities;

public sealed class ProductEntity : BaseEntity
{
    public ProductEntity() { }
    public ProductEntity(string name, string description, decimal price, int initialStock, int productTypeId, int brandId, int? photoId = null)
    {
        SetName(name);
        SetDescription(description);
        ChangeUnitPrice(price);
        IncreaseStock(initialStock);
        ProductTypeId = productTypeId;
        BrandId = brandId;
        PhotoId = photoId ?? 0;
        CreatedOn = DateTime.UtcNow;
    }

    #region Properties
    public string Name { get; private set; } = default!;
    public string NameNormalized { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int QuantityInStock { get; private set; }
    public int ProductTypeId { get; private set; }
    public ProductTypeEntity? ProductType { get; private set; }
    public int BrandId { get; private set; }
    public BrandEntity? Brand { get; private set; }
    public int PhotoId { get; private set; }
    public PhotoEntity? Photo { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    #endregion

    #region Business Logic
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required.");
        var collapsed = CollapseSpaces(name.Trim());
        if (collapsed.Length > 64) throw new ArgumentException("Name too long (max 64).");

        Name = collapsed;
        NameNormalized = collapsed.ToUpperInvariant();
        Touch();
    }

    public void ChangeUnitPrice(decimal newPrice)
    {
        if (newPrice < 0m) throw new ArgumentException("Price cannot be negative.");
        if (newPrice == UnitPrice) return;
        UnitPrice = newPrice;
        Touch();
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        QuantityInStock += quantity;
        Touch();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (quantity > QuantityInStock) throw new ArgumentException("Insufficient stock.");
        QuantityInStock -= quantity;
        Touch();
    }

    public void SetPhoto(int photoId, PhotoEntity? photo = null)
    {
        if (photoId <= 0) throw new ArgumentException("Invalid photo id.");
        PhotoId = photoId;
        Photo = photo;
        Touch();
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.");
        Description = description.Trim();
        Touch();
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
