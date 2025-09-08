namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public sealed class BasketEntityConfig : IEntityTypeConfiguration<BasketEntity>
{
    public void Configure(EntityTypeBuilder<BasketEntity> builder)
    {
        builder.Property(b => b.PaymentIntentId).HasMaxLength(64).IsUnicode(false);
        builder.Property(b => b.ClientSecret).HasMaxLength(256).IsUnicode(false);
        builder.Property(b => b.Discount).HasPrecision(18, 2);
        builder.Property(a => a.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
        builder.HasOne(b => b.Coupon).WithMany().HasForeignKey(b => b.CouponId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.BasketItems).WithOne(i => i.Basket).HasForeignKey(i => i.BasketId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o => o.BasketItems).HasField("_basketItems").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
