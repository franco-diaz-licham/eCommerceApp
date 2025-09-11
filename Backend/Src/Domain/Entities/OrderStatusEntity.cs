namespace Backend.Src.Domain.Entities;

public class OrderStatusEntity : BaseEntity
{
    private OrderStatusEntity() { }
    public OrderStatusEntity(string name)
    {
        SetName(name);
        IsActive = true;
    }

    #region
    public string Name { get; private set; } = default!;
    public string NameNormalized { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    #endregion

    #region
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException($"{nameof(name)} is required.");
        var trimmed = name.Trim();
        if (trimmed.Length > 64) throw new ArgumentException($"{nameof(name)} too long (max 64).");

        Name = CollapseSpaces(trimmed);
        NameNormalized = Name.ToUpperInvariant();
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}
