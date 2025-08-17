namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class OrderStatusEntityConfig : IEntityTypeConfiguration<OrderStatusEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusEntity> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(64);
        builder.Property(s => s.NameNormalized).IsRequired().HasMaxLength(64);
        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.CreatedOn).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
