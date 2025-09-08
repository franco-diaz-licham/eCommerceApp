namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class ProductEntityConfig : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        // Strings
        builder.Property(p => p.Name).IsRequired().HasMaxLength(64);
        builder.Property(p => p.NameNormalized).IsRequired().HasMaxLength(64);
        builder.Property(p => p.UnitPrice).HasPrecision(18, 2);
        builder.Property(p => p.QuantityInStock).IsRequired();
        builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.ProductType).WithMany().HasForeignKey(p => p.ProductTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Brand).WithMany().HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Photo).WithMany().HasForeignKey(p => p.PhotoId).OnDelete(DeleteBehavior.NoAction);

    }
} 