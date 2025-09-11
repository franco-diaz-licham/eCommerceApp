namespace Backend.Src.Domain.Entities;

public class BrandEntity : BaseEntity
{
    private BrandEntity() { }
    public BrandEntity(string name)
    {
        SetName(name);
        IsActive = true;
    }

    #region Properties
    public string Name { get; private set; } = default!;
    public string NameNormalized { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    #endregion

    #region Business logic
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException($"{nameof(name)} is required.");
        var collapsed = CollapseSpaces(name.Trim());
        if (collapsed.Length > 64) throw new ArgumentException($"{nameof(name)} too long.");

        Name = collapsed;
        NameNormalized = collapsed.ToUpperInvariant();
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
