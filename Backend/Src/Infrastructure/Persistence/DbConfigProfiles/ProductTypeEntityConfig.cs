namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class ProductTypeEntityConfig : IEntityTypeConfiguration<ProductTypeEntity>
{
    public void Configure(EntityTypeBuilder<ProductTypeEntity> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(64);
        builder.Property(t => t.NameNormalized).IsRequired().HasMaxLength(64);
        builder.Property(t => t.IsActive).HasDefaultValue(true);
    }
}
