namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class OrderEntityConfig : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        // Properties
        builder.Property(o => o.Subtotal).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.PaymentIntentId).IsRequired().HasMaxLength(64);
        builder.Property(o => o.UserId).IsRequired().HasMaxLength(450);
        builder.Property(o => o.LastProcessedStripeEventId).HasMaxLength(128);
        builder.Property(o => o.OrderStatusId).HasDefaultValue((int)OrderStatusEnum.Pending);

        // Relationships
        builder.HasOne(o => o.OrderStatus).WithMany().HasForeignKey(o => o.OrderStatusId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o => o.OrderItems).WithOne(i => i.Order).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o => o.OrderItems).HasField("_orderItems").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(o => o.PaymentIntentId).IsUnique();
        builder.OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(a => a.RecipientName).IsRequired().HasMaxLength(64);
            sa.Property(a => a.Line1).IsRequired().HasMaxLength(128);
            sa.Property(a => a.Line2).HasMaxLength(128);
            sa.Property(a => a.City).IsRequired().HasMaxLength(12);
            sa.Property(a => a.State).IsRequired().HasMaxLength(12);
            sa.Property(a => a.PostalCode).IsRequired().HasMaxLength(4);
            sa.Property(a => a.Country).IsRequired().HasMaxLength(56);
        });
        builder.OwnsOne(o => o.PaymentSummary, ps =>
        {
            ps.Property(b => b.Brand).HasMaxLength(64);
        });
        builder.Navigation(o => o.ShippingAddress).IsRequired();
        builder.Navigation(o => o.PaymentSummary).IsRequired();
    }
}
