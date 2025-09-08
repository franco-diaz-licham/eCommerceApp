namespace Backend.Src.Infrastructure;

public class DataContext : IdentityDbContext<UserEntity>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public required DbSet<AddressEntity> Addresses { get; set; }
    public required DbSet<OrderEntity> Orders { get; set; }
    public required DbSet<OrderItemEntity> OrdersItems { get; set; }
    public required DbSet<OrderStatusEntity> OrderStatuses { get; set; }
    public required DbSet<ProductEntity> Products { get; set; }
    public required DbSet<BrandEntity> Brands { get; set; }
    public required DbSet<ProductTypeEntity> ProductTypes { get; set; }
    public required DbSet<PhotoEntity> Photos { get; set; }
    public required DbSet<BasketEntity> Baskets { get; set; }
    public required DbSet<BasketItemEntity> BasketItems { get; set; }
    public required DbSet<CouponEntity> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure relations and apply all configurations
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
