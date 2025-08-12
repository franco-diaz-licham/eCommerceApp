namespace Backend.Src.Infrastructure;

public class DataContext : IdentityDbContext<UserEntity>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public required DbSet<AddressEntity> Addresses { get; set; }
    public required DbSet<OrderEntity> Orders { get; set; }
    public required DbSet<OrderItemEntity> OrdersItems { get; set; }
    public required DbSet<OrderStatusEntity> OrderStatuses { get; set; }
    public required DbSet<ProductEntity> Products { get; set; }
    public required DbSet<ProductItemEntity> ProductItems { get; set; }
    public required DbSet<BrandEntity> Brands { get; set; }
    public required DbSet<ProductTypeEntity> ProductTypes { get; set; }
    public required DbSet<PhotoEntity> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure relations
        base.OnModelCreating(builder);

        builder.Entity<OrderEntity>().HasMany(x => x.ProductItems).WithMany(x => x.Orders).UsingEntity<OrderItemEntity>(
            x => x.HasOne(x => x.ProductItem).WithMany().HasForeignKey(x => x.ProductItemId).OnDelete(DeleteBehavior.NoAction).IsRequired(),
            x => x.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade).IsRequired(),
            x => x.HasKey(x => new { x.OrderId, x.ProductItemId })
        );

        // Configure all datetimes to be UTC, Before saving to DB and After reading from DB
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        foreach (var entityType in builder.Model.GetEntityTypes())
            foreach (var property in entityType.GetProperties().Where(p => p.ClrType == typeof(DateTime)))
                property.SetValueConverter(dateTimeConverter);
    }
}
