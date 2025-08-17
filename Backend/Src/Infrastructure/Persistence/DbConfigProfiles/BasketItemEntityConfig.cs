namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public sealed class BasketItemEntityConfig : IEntityTypeConfiguration<BasketItemEntity>
{
    public void Configure(EntityTypeBuilder<BasketItemEntity> builder)
    {
        builder.HasKey(i => new { i.BasketId, i.ProductId });
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
