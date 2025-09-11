namespace BackendTests.IntegrationTests.Application;

public class OrderServiceIntegrationTests : SqlDbTestBase
{
    private BasketEntity MakeBasketWithItems((ProductEntity p, int qty)[] items, string? piId = "pi_123", string? cs = "cs_123", decimal discount = 0m)
    {
        var b = new BasketEntity();
        if (piId is not null) b.SetPaymentIntentId(piId);
        if (cs is not null) b.SetClientSecret(cs);
        foreach (var (p, qty) in items) b.AddItem(p.Id, p.UnitPrice, qty);
        if (discount > 0) b.SetDiscount(discount); // if your entity enforces a method, switch to it.
        return b;
    }

    private OrderService Service(DataContext context) => new(context, Mapper);

    [Fact]
    public async Task GetAsync_ShouldReturnDto_WhenIdAndEmailMatch()
    {
        // Arrange
        using var context = CreateContext();
        var orderStatus = new OrderStatusEntity("Pending");
        await SeedAsync(orderStatus);
        var service = Service(context);
        var product = await CreateProductAsync();
        var order = new OrderEntity(
            userEmail: "user@example.com",
            shipping: new ShippingAddress("L1", "L2", "City", "State", "2000", "AU"),
            paymentIntentId: "piX",
            deliveryFee: 5m,
            subtotal: 50m,
            discount: 0m,
            summary: new PaymentSummary { Last4 = 7890, Brand = "Puma", ExpMonth = 11, ExpYear = 2025 },
            items: new() { new OrderItemEntity(product.Id, product.Name, product.UnitPrice, 2) }
        );

        await SeedAsync(order);

        // Act
        var result = await service.GetAsync(order.Id, "user@example.com");
        
        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(order.Id);
        result.Value.UserEmail.Should().Be("user@example.com");
        result.Value.OrderItems.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenEmailDoesNotMatch()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);

        // Act
        var result = await service.GetAsync(999, "user@example.com");

        // Assert
        result.Value.Should().BeNull();
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Order could not be found");
    }

    //// ------------------------- GetAllAsync --------------------------------

    //[Fact]
    //public async Task GetAllAsync_ShouldReturnPagedOrders()
    //{
    //    // Seed a few orders
    //    var o1 = new OrderEntity("a@x.com", new ShippingAddress("L1", "", "", "", "", "AU"), "pi1", 5m, 50m, 0m, new PaymentSummary("pm", "ch", "s", DateTime.UtcNow), new());
    //    var o2 = new OrderEntity("b@x.com", new ShippingAddress("L1", "", "", "", "", "AU"), "pi2", 0m, 150m, 0m, new PaymentSummary("pm", "ch", "s", DateTime.UtcNow), new());
    //    var o3 = new OrderEntity("c@x.com", new ShippingAddress("L1", "", "", "", "", "AU"), "pi3", 0m, 200m, 10m, new PaymentSummary("pm", "ch", "s", DateTime.UtcNow), new());

    //    await SeedAsync(o1, o2, o3);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);
    //    var specs = new BaseQuerySpecs
    //    {
    //        PageNumber = 1,
    //        PageSize = 2,
    //        OrderBy = "idAsc" // <-- adjust to match your OrderSortProvider
    //    };

    //    var res = await svc.GetAllAsync(specs);

    //    res.IsSuccess.Should().BeTrue();
    //    res.Value.Should().HaveCount(2);
    //    res.Type.Should().Be(ResultTypeEnum.Success);
    //}

    //// --------------------- CreateOrderAsync: Guards -----------------------

    //[Fact]
    //public async Task CreateOrderAsync_ShouldFail_WhenBasketEmpty()
    //{
    //    var basket = new BasketEntity { PaymentIntentId = "pi_empty", ClientSecret = "cs" };
    //    await SeedAsync(basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);
    //    res.IsSuccess.Should().BeFalse();
    //    res.Type.Should().Be(ResultTypeEnum.Invalid);
    //    res.Error!.Message.Should().Contain("Basket is empty");
    //}

    //[Fact]
    //public async Task CreateOrderAsync_ShouldFail_WhenBasketHasNoPaymentIntent()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku1", "P1", 30m, 5, brand, type);

    //    var basket = MakeBasketWithItems(new[] { (p, 1) }, piId: null, cs: "cs-any");
    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);
    //    res.IsSuccess.Should().BeFalse();
    //    res.Type.Should().Be(ResultTypeEnum.Invalid);
    //    res.Error!.Message.Should().Contain("Invalid basket payment intent");
    //}

    //[Fact]
    //public async Task CreateOrderAsync_ShouldFail_WhenAnyBasketItemHasZeroQuantity()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku2", "P2", 20m, 5, brand, type);

    //    // Manually craft a zero-qty item (bypassing AddItem guard if any)
    //    var basket = new BasketEntity { PaymentIntentId = "piZ", ClientSecret = "csZ" };
    //    basket.BasketItems.Add(new BasketItemEntity { ProductId = p.Id, UnitPrice = p.UnitPrice, Quantity = 0 });

    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);
    //    res.IsSuccess.Should().BeFalse();
    //    res.Type.Should().Be(ResultTypeEnum.Invalid);
    //    res.Error!.Message.Should().Contain("zero quantity");
    //}

    //// ----------- CreateOrderAsync: Insufficient stock rollback ------------

    //[Fact]
    //public async Task CreateOrderAsync_ShouldRollback_WhenInsufficientStock()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku3", "P3", 40m, stock: 1, brand, type);

    //    var basket = MakeBasketWithItems(new[] { (p, 5) }, piId: "pi_ins", cs: "cs_ins");
    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);

    //    res.IsSuccess.Should().BeFalse();
    //    res.Type.Should().Be(ResultTypeEnum.Invalid);
    //    res.Error!.Message.Should().Contain("Insufficient stock");

    //    // Stock should remain unchanged due to rollback
    //    var after = await ctx.Products.AsNoTracking().SingleAsync(x => x.Id == p.Id);
    //    after.QuantityInStock.Should().Be(1);

    //    // No order should be inserted
    //    var anyOrder = await ctx.Orders.AsNoTracking().AnyAsync(o => o.PaymentIntentId == "pi_ins");
    //    anyOrder.Should().BeFalse();
    //}

    //// --------------- CreateOrderAsync: Happy path (create) ----------------

    //[Fact]
    //public async Task CreateOrderAsync_ShouldCreateOrder_DecrementStock_ApplyDeliveryFeeRule()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku4", "P4", 30m, stock: 10, brand, type); // price=30

    //    // subtotal = 60 (< 100) & discount = 0 → deliveryFee = 5
    //    var basket = MakeBasketWithItems(new[] { (p, 2) }, piId: "pi_ok", cs: "cs_ok", discount: 0m);
    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);

    //    res.IsSuccess.Should().BeTrue();
    //    res.Type.Should().Be(ResultTypeEnum.Created);
    //    res.Value!.UserEmail.Should().Be("user@x.com");
    //    res.Value!.DeliveryFee.Should().Be(5m);
    //    res.Value!.Items.Should().ContainSingle(i => i.ProductId == p.Id && i.Quantity == 2);

    //    // Stock should decrement by 2
    //    var after = await ctx.Products.AsNoTracking().SingleAsync(x => x.Id == p.Id);
    //    after.QuantityInStock.Should().Be(8);
    //}

    //[Fact]
    //public async Task CreateOrderAsync_ShouldSetZeroDelivery_WhenSubtotalAtLeast100()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku5", "P5", 50m, stock: 10, brand, type); // 50 * 2 = 100

    //    var basket = MakeBasketWithItems(new[] { (p, 2) }, piId: "pi_free", cs: "cs_free", discount: 0m);
    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);

    //    res.IsSuccess.Should().BeTrue();
    //    res.Value!.DeliveryFee.Should().Be(0m);
    //}

    //[Fact]
    //public async Task CreateOrderAsync_ShouldSetZeroDelivery_WhenDiscountApplied()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p = MakeProduct("sku6", "P6", 30m, stock: 10, brand, type);

    //    // subtotal = 60 but discount > 0 → deliveryFee = 0
    //    var basket = MakeBasketWithItems(new[] { (p, 2) }, piId: "pi_disc", cs: "cs_disc", discount: 5m);
    //    await SeedAsync(brand, type, p, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("L1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm", "ch", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);

    //    res.IsSuccess.Should().BeTrue();
    //    res.Value!.DeliveryFee.Should().Be(0m);
    //}

    //// --------------- CreateOrderAsync: Upsert (update existing) -----------

    //[Fact]
    //public async Task CreateOrderAsync_ShouldAppendItemsAndUpdateFields_WhenOrderExistsForPaymentIntent()
    //{
    //    var brand = new BrandEntity("B");
    //    var type = new ProductTypeEntity("T");
    //    var p1 = MakeProduct("sku7", "P7", 20m, stock: 10, brand, type);
    //    var p2 = MakeProduct("sku8", "P8", 15m, stock: 10, brand, type);

    //    // Existing order tied to same payment intent
    //    var existing = new OrderEntity(
    //        "user@x.com",
    //        new ShippingAddress("Old", "", "", "", "", "AU"),
    //        "pi_same",
    //        deliveryFee: 0m,
    //        subtotal: 20m,
    //        discount: 0m,
    //        summary: new PaymentSummary("pm-old", "ch-old", "requires_action", DateTime.UtcNow),
    //        items: new() { new OrderItemEntity(p1.Id, p1.Name, p1.UnitPrice, 1) }
    //    );

    //    // Basket with new items on same intent
    //    var basket = MakeBasketWithItems(new[] { (p2, 2) }, piId: "pi_same", cs: "cs_same", discount: 0m);

    //    await SeedAsync(brand, type, p1, p2, existing, basket);

    //    using var ctx = CreateContext();
    //    var svc = Svc(ctx);

    //    var dto = new OrderCreateDto
    //    {
    //        BasketId = basket.Id,
    //        UserEmail = "user@x.com",
    //        ShippingAddress = new ShippingAddressDto("NewL1", "", "City", "State", "2000", "AU"),
    //        PaymentSummary = new PaymentSummaryDto("pm-new", "ch-new", "succeeded", DateTime.UtcNow)
    //    };

    //    var res = await svc.CreateOrderAsync(dto);

    //    res.IsSuccess.Should().BeTrue();
    //    res.Type.Should().Be(ResultTypeEnum.Created);

    //    // Items appended (existing p1 + new p2)
    //    res.Value!.Items.Should().HaveCount(2);
    //    res.Value!.Items.Any(i => i.ProductId == p1.Id).Should().BeTrue();
    //    res.Value!.Items.Any(i => i.ProductId == p2.Id && i.Quantity == 2).Should().BeTrue();

    //    // Shipping/charges/summary updated
    //    res.Value!.ShippingAddress.Line1.Should().Be("NewL1");
    //    res.Value!.PaymentSummary.Should().NotBeNull();
    //}
}
