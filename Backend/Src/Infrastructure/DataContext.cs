namespace Backend.Src.Infrastructure;

public class DataContext : IdentityDbContext<UserEntity>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrdersItems { get; set; }
    public DbSet<OrderStatusEntity> OrderStatuses { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<BrandEntity> Brands { get; set; }
    public DbSet<ProductTypeEntity> ProductTypes { get; set; }
    public DbSet<PhotoEntity> Photos { get; set; }
    public DbSet<BasketEntity> Baskets { get; set; }
    public DbSet<BasketItemEntity> BasketItems { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure relations
        base.OnModelCreating(builder);

        // Apply all custom configurations
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        TouchTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        TouchTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void TouchTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var e in ChangeTracker.Entries<BaseEntity>())
        {
            if (e.State == EntityState.Added) e.Entity.Created(now);
            else if (e.State == EntityState.Modified) e.Entity.Updated(now);
        }
    }
}
