namespace Backend.Src.Domain.Entities;

public sealed class ProductEntity : BaseEntity
{
    private ProductEntity() { }
    public ProductEntity(
        string name, 
        string description, 
        decimal unitPrice, 
        int quantityInStock, 
        int productTypeId, 
        int brandId, 
        int photoId,
        BrandEntity? brand = null,
        ProductTypeEntity? type = null)
    {
        SetName(name);
        SetDescription(description);
        ChangeUnitPrice(unitPrice);
        IncreaseStock(quantityInStock);
        ProductTypeId = productTypeId;
        BrandId = brandId;
        PhotoId = photoId;
        Brand = brand;
        ProductType = type;
    }

    public ProductEntity(
        string name,
        string description,
        decimal unitPrice,
        int quantityInStock,
        int productTypeId,
        int brandId)
    {
        SetName(name);
        SetDescription(description);
        ChangeUnitPrice(unitPrice);
        IncreaseStock(quantityInStock);
        ProductTypeId = productTypeId;
        BrandId = brandId;
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
    #endregion

    #region Business Logic
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException($"{nameof(name)} is required.");
        var collapsed = CollapseSpaces(name.Trim());
        if (collapsed.Length > 64) throw new ArgumentException($"{nameof(name)} too long (max 64).");

        Name = collapsed;
        NameNormalized = collapsed.ToUpperInvariant();
    }

    public void ChangeUnitPrice(decimal price)
    {
        if (price < 0m) throw new ArgumentException($"{nameof(price)} cannot be negative.");
        if (price == UnitPrice) return;
        UnitPrice = price;
    }

    /// <summary>
    /// Updates the stock value.
    /// </summary>
    public void SetStock(int quantity)
    {
        int val = quantity - QuantityInStock;
        if (val > 0) IncreaseStock(val);
        if (val < 0) DecreaseStock(-1 * val);
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException($"{nameof(quantity)} must be positive.");
        checked { QuantityInStock += quantity; };
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException($"{nameof(quantity)} must be positive.");
        if (quantity > QuantityInStock) throw new ArgumentException("Insufficient stock.");
        QuantityInStock -= quantity;
    }

    public void SetPhoto(int photoId, PhotoEntity? photo = null)
    {
        if (photoId <= 0) throw new ArgumentException($"{nameof(photoId)} is invalid.");
        PhotoId = photoId;
        Photo = photo;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException($"{nameof(description)} is required.");
        Description = description.Trim();
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}
