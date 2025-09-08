namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public sealed class BrandEntityConfig : IEntityTypeConfiguration<BrandEntity>
{
    public void Configure(EntityTypeBuilder<BrandEntity> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(64);
        builder.Property(b => b.NameNormalized).IsRequired().HasMaxLength(64);
        builder.Property(b => b.IsActive).HasDefaultValue(true);
        builder.Property(b => b.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
    }
}
