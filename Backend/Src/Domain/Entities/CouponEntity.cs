namespace Backend.Src.Domain.Entities;

public class CouponEntity : BaseEntity
{
    private CouponEntity() { }
    private CouponEntity(string name, string remoteId, string code)
    {
        SetName(name);
        SetPromotionCode(code);
        SetRemoteId(remoteId);
        IsActive = true;
    }

    public static CouponEntity CreateAmountOff(string name, string remoteId, string code, decimal amountOff)
    {
        var c = new CouponEntity(name, remoteId, code);
        c.SetAmountOff(amountOff);
        return c;
    }

    public static CouponEntity CreatePercentOff(string name, string remoteId, string code, decimal percentOff)
    {
        var c = new CouponEntity(name, remoteId, code);
        c.SetPercentOff(percentOff);
        return c;
    }

    #region Properties
    public string Name { get; private set; } = default!;
    public string? RemoteId { get; private set; }
    public string NameNormalized { get; private set; } = default!;
    public decimal? AmountOff { get; private set; }
    public decimal? PercentOff { get; private set; }
    public string PromotionCode { get; private set; } = default!;
    public string PromotionCodeNormalized { get; private set; } = default!;
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

    public void SetPromotionCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException($"{nameof(code)} is required.");
        var collapsed = CollapseSpaces(code.Trim());
        if (collapsed.Length > 64) throw new ArgumentException($"{nameof(code)} too long.");

        PromotionCode = collapsed;
        PromotionCodeNormalized = collapsed.ToUpperInvariant();
    }

    public void SetRemoteId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException($"{nameof(id)} is required.");
        RemoteId = id.Trim();
    }

    public void SetAmountOff(decimal amount)
    {
        if (amount < 0m) throw new ArgumentException($"{nameof(amount)} cannot be negative.");
        AmountOff = amount;
        PercentOff = null;
    }

    public void SetPercentOff(decimal percent)
    {
        if (percent < 0m || percent > 100m) throw new ArgumentException($"{nameof(percent)} must be between 0 and 100.");
        PercentOff = percent;
        AmountOff = null;
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;    }

    /// <summary>
    /// Calculates the discount for a given subtotal and clamps it so it never exceeds subtotal. Throws if coupon is inactive or misconfigured.
    /// </summary>
    public decimal CalculateDiscount(decimal subtotal)
    {
        if (!IsActive) throw new ArgumentException($"{nameof(Coupon)} inactive.");
        if (subtotal < 0m) throw new ArgumentException($"{nameof(subtotal)} cannot be negative.");

        if (AmountOff is not null) return Math.Min((decimal)AmountOff, subtotal);
        if (PercentOff is not null) return Math.Min(Math.Round(subtotal * ((decimal)PercentOff / 100m), 2, MidpointRounding.ToPositiveInfinity), subtotal);

        throw new ArgumentException($"{nameof(Coupon)} must have either AmountOff or PercentOff.");
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}
