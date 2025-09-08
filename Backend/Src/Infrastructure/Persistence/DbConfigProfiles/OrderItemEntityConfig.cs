namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class OrderItemEntityConfig : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        // Relationships
        builder.HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(128);
        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        builder.Property(oi => oi.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
    }
}
