namespace Backend.Src.Infrastructure.Persistence.DbConfigProfiles;

public class PhotoEntityConfig : IEntityTypeConfiguration<PhotoEntity>
{
    public void Configure(EntityTypeBuilder<PhotoEntity> builder)
    {
        builder.Property(p => p.FileName).IsRequired().HasMaxLength(128);
        builder.Property(p => p.PublicId).IsRequired().HasMaxLength(128);
        builder.Property(p => p.PublicUrl).IsRequired();
        builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
    }
}
