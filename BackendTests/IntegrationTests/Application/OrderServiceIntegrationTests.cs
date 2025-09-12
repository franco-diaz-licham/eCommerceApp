namespace BackendTests.IntegrationTests.Application;

public class OrderServiceIntegrationTests : SqlDbTestBase
{
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
    public async Task GetAsync_ShouldReturnNull_WhenIdOrEmailDoNotMatch()
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

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedOrders()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var orderStatus = new OrderStatusEntity("Pending");
        await SeedAsync(orderStatus);

        var address1 = new ShippingAddress("Line1", "Line2", "Test", "Test", "Tes", "AU");
        var address2 = new ShippingAddress("Line1", "Line2", "Test", "Test", "Tes", "AU");
        var address3 = new ShippingAddress("Line1", "Line2", "Test", "Test", "Tes", "AU");
        var payment1 = new PaymentSummary { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 };
        var payment2 = new PaymentSummary { Last4 = 1224, Brand = "Puma", ExpMonth = 1, ExpYear = 2026 };
        var payment3 = new PaymentSummary { Last4 = 1134, Brand = "Puma", ExpMonth = 1, ExpYear = 2027 };
        var order1 = new OrderEntity("a@x.com", address1, "pi1", 5m, 50m, 0m, payment1, new());
        var order2 = new OrderEntity("b@x.com", address2, "pi2", 0m, 150m, 0m, payment2, new());
        var order3 = new OrderEntity("c@x.com", address3, "pi3", 0m, 200m, 10m, payment3, new());
        var specs = new BaseQuerySpecs { PageNumber = 1, PageSize = 2, OrderBy = "createdDesc" };
        await SeedAsync(order1, order2, order3);

        // Act
        var result = await service.GetAllAsync(specs);
        
        // Asswer
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Type.Should().Be(ResultTypeEnum.Success);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFail_WhenNoOrdersFound()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var specs = new BaseQuerySpecs { PageNumber = 1, PageSize = 2, OrderBy = "createdDesc" };

        // Act
        var result = await service.GetAllAsync(specs);

        // Asswer
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error?.Message.Should().Contain("Orders not found");
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldFail_WhenBasketEmpty()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basket = new BasketEntity( "pi_empty", "cs" );
        await SeedAsync(basket);
        var orderCreateDto = new OrderCreateDto
        {
            BasketId = basket.Id,
            UserEmail = "user@x.com",
            ShippingAddress = new AddressDto { Line1 = "L1", Line2 = "", City = "City", State = "State", PostalCode = "2000", Country = "AU" },
            PaymentSummary = new PaymentSummaryDto { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 }
        };

        // Act
        var res = await service.CreateOrderAsync(orderCreateDto);
        
        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Type.Should().Be(ResultTypeEnum.Invalid);
        res.Error!.Message.Should().Contain("Basket is empty");
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldFail_WhenBasketHasNoPaymentIntent()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();
        var basket = await MakeBasketWithItems(new[] { (product, 1) }, piId: null, clientSecret: "cs-any");
        var dto = new OrderCreateDto
        {
            BasketId = basket.Id,
            UserEmail = "user@x.com",
            ShippingAddress = new AddressDto { Line1 = "L1", Line2 = "", City = "City", State = "State", PostalCode = "2000", Country = "AU" },
            PaymentSummary = new PaymentSummaryDto { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 }
        };

        // Act
        var res = await service.CreateOrderAsync(dto);
        
        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Type.Should().Be(ResultTypeEnum.Invalid);
        res.Error!.Message.Should().Contain("Invalid basket payment intent");
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldFail_WhenUnableToSaveChanges()
    {
        // Arrange
        using var context = CreateContext();
        var ctxMock = new Mock<DataContext>(Options) { CallBase = true };
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object);

        var product = await CreateProductAsync();
        var basket = await MakeBasketWithItems(new[] { (product, 1) }, piId: null, clientSecret: "cs-any");
        var dto = new OrderCreateDto
        {
            BasketId = basket.Id,
            UserEmail = "user@x.com",
            ShippingAddress = new AddressDto { Line1 = "L1", Line2 = "", City = "City", State = "State", PostalCode = "2000", Country = "AU" },
            PaymentSummary = new PaymentSummaryDto { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 }
        };

        // Act
        var result = await service.CreateOrderAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error?.Message.Contains("Order could not be created");
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldRollback_WhenDomainLogicThrowsInsufficientStock()
    {
        // Arrange: replicate insufficient item stock
        using var context = CreateContext();
        var service = Service(context);

        var status = new OrderStatusEntity("Pending");
        var brand = new BrandEntity("B");
        var type = new ProductTypeEntity("T");
        var photo = new PhotoEntity("photo1", "123456789", "https://test/com/photo1.jpg");

        await SeedAsync(status, brand, type, photo);

        var product = new ProductEntity("sku3", "P3", 40m, 1, type.Id, brand.Id, photoId: photo.Id);
        await SeedAsync(product);

        var basket = await MakeBasketWithItems(new[] { (product, 5) }, piId: "pi_ins", clientSecret: "cs_ins");
        var orderCreateDto = new OrderCreateDto
        {
            BasketId = basket.Id,
            UserEmail = "user@x.com",
            ShippingAddress = new AddressDto { Line1 = "L1", Line2 = "", City = "City", State = "State", PostalCode = "2000", Country = "AU" },
            PaymentSummary = new PaymentSummaryDto { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 }
        };

        // Act
        var result = await service.CreateOrderAsync(orderCreateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("There was a problem creating the order");

        var after = await context.Products.AsNoTracking().SingleAsync(x => x.Id == product.Id);
        after.QuantityInStock.Should().Be(1);

        var anyOrder = await context.Orders.AsNoTracking().AnyAsync(o => o.PaymentIntentId == "pi_ins");
        anyOrder.Should().BeFalse();
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateNewOrder()
    {
        // Act
        using var context = CreateContext();
        var service = Service(context);

        var status = new OrderStatusEntity("Pending");
        var brand = new BrandEntity("B");
        var type = new ProductTypeEntity("T");
        var photo = new PhotoEntity("photo1", "123456789", "https://test/com/photo1.jpg");
        await SeedAsync(status, brand, type, photo);

        var p1 = new ProductEntity("sku7", "P7", 20m, 10, type.Id, brand.Id, photoId: photo.Id);
        var p2 = new ProductEntity("sku8", "P8", 15m, 10, type.Id, brand.Id, photoId: photo.Id);
        await SeedAsync(p1, p2);

        var basket = await MakeBasketWithItems(new[] { (p1, 2), (p2, 2) }, piId: "pi_same", clientSecret: "cs_same", discount: 0m);

        var dto = new OrderCreateDto
        {
            BasketId = basket.Id,
            UserEmail = "user@x.com",
            ShippingAddress = new AddressDto { Line1 = "L1", Line2 = "", City = "City", State = "State", PostalCode = "2000", Country = "AU" },
            PaymentSummary = new PaymentSummaryDto { Last4 = 1233, Brand = "Puma", ExpMonth = 1, ExpYear = 2025 }
        };

        // Act
        var res = await service.CreateOrderAsync(dto);

        res.IsSuccess.Should().BeTrue();
        res.Type.Should().Be(ResultTypeEnum.Created);
        res.Value!.OrderItems.Should().HaveCount(2);
        res.Value!.OrderItems.Any(i => i.ProductId == p1.Id && i.Quantity == 2).Should().BeTrue();
        res.Value!.OrderItems.Any(i => i.ProductId == p2.Id && i.Quantity == 2).Should().BeTrue();
        res.Value!.ShippingAddress.Line1.Should().Be("L1");
        res.Value!.PaymentSummary.Should().NotBeNull();
    }
}
