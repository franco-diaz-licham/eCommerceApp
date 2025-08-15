namespace Backend.Src.Domain.Entities;

public class BrandEntity : BaseEntity
{
    private BrandEntity() { }
    public BrandEntity(string name)
    {
        SetName(name);
        IsActive = true;
        CreatedOn = DateTime.UtcNow;
    }

    #region Properties
    public string Name { get; private set; } = default!;
    public string NameNormalized { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    #endregion

    #region Business logic
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Name is required.");
        var collapsed = CollapseSpaces(name.Trim());
        if (collapsed.Length > 64) throw new ArgumentException("Name too long (max 64).");

        Name = collapsed;
        NameNormalized = collapsed.ToUpperInvariant();
        Touch();
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
