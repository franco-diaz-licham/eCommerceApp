namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public sealed class CouponEntityConfig : IEntityTypeConfiguration<CouponEntity>
{
    public void Configure(EntityTypeBuilder<CouponEntity> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(64);
        builder.Property(c => c.NameNormalized).IsRequired().HasMaxLength(64);
        builder.Property(c => c.PromotionCode).IsRequired().HasMaxLength(64);
        builder.Property(c => c.PromotionCodeNormalized).IsRequired().HasMaxLength(64);
        builder.Property(c => c.RemoteId).HasMaxLength(128);
        builder.Property(c => c.AmountOff).HasPrecision(18, 2);
        builder.Property(c => c.PercentOff).HasPrecision(5, 2);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
    }
}
