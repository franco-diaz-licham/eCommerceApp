namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public sealed class AddressEntityConfig : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        builder.Property(a => a.Line1).IsRequired().HasMaxLength(128);
        builder.Property(a => a.Line2).HasMaxLength(128);
        builder.Property(a => a.City).IsRequired().HasMaxLength(12);
        builder.Property(a => a.State).IsRequired().HasMaxLength(12);
        builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(4);
        builder.Property(a => a.Country).IsRequired().HasMaxLength(56);
    }
}
