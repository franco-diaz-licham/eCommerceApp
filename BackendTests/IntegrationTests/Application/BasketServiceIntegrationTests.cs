namespace BackendTests.IntegrationTests.Application;

public class BasketServiceIntegrationTests : SqlDbTestBase
{
    private async Task<int> CreateBasketAsync(BasketEntity basket, DataContext context)
    {
        context.Baskets.Add(basket);
        await context.SaveChangesAsync();
        return basket.Id;
    }

    private BasketService Service(DataContext context, Mock<IPaymentGateway> paymentsMock = null!)
    {
        var logger = new Mock<ILogger<BasketService>>().Object;
        var payments = paymentsMock?.Object ?? new Mock<IPaymentGateway>().Object;
        return new BasketService(context, Mapper, logger, payments);
    }

    [Fact]
    public async Task GetBasketAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();

        var basket = new BasketEntity();
        basket.AddItem(productId: product.Id, unitPrice: product.UnitPrice, quantity: 2);
        var basketId = await CreateBasketAsync(basket, context);

        // Act
        var result = await service.GetBasketAsync(basketId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Type.Should().Be(ResultTypeEnum.Success);
        result.Value!.BasketItems.Should().ContainSingle(i => i.ProductId == product.Id && i.Quantity == 2);
    }

    [Fact]
    public async Task GetBasketAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);

        // Act
        var result = await service.GetBasketAsync(99999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error!.Message.Should().Contain("Basket not found");
    }

    [Fact]
    public async Task CreateBasketAsync_ShouldCreateAndReturnDto()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);

        // Act
        var result = await service.CreateBasketAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Type.Should().Be(ResultTypeEnum.Created);
        result.Value!.Id.Should().BeGreaterThan(0);
        var exists = await context.Baskets.AsNoTracking().AnyAsync(b => b.Id == result.Value.Id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBasketAsync_ShouldFail_WhenUnableToSaveChanges()
    {
        // Arrange
        using var context = CreateContext();
        var ctxMock = new Mock<DataContext>(Options) { CallBase = true };
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object);

        // Act
        var result = await service.CreateBasketAsync();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error?.Message.Contains("Basket could not be created");
    }

    [Fact]
    public async Task AddItemAsync_ShouldFail_WhenQuantityNotPositive()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var dto = new BasketItemDto{ BasketId = 1, ProductId = 1, Quantity = 0 };

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Quantity must be positive");
    }

    [Fact]
    public async Task AddItemAsync_ShouldFail_WhenBasketMissing()
    {
        // Assert
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();
        var dto = new BasketItemDto{ BasketId = 123456, ProductId = product.Id, Quantity = 1 };

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error!.Message.Should().Contain("Basket not found");
    }

    [Fact]
    public async Task AddItemAsync_ShouldFail_WhenProductMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basketId = await CreateBasketAsync(new BasketEntity(), context);
        var dto = new BasketItemDto{ BasketId = basketId, ProductId = 999999, Quantity = 1 };

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error!.Message.Should().Contain("Product not found");
    }

    [Fact]
    public async Task AddItemAsync_ShouldAddNewItem_WhenInputValid()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();
        var basketId = await CreateBasketAsync(new BasketEntity(), context);
        var dto = new BasketItemDto { BasketId = basketId, ProductId = product.Id, Quantity = 3 };

        // Act
        var result = await service.AddItemAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.BasketItems.Should().ContainSingle(i => i.ProductId == product.Id && i.Quantity == 3);
    }

    [Fact]
    public async Task AddItemAsync_ShouldIncreaseQuantity_WhenItemAlreadyExists()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();
        var basket = new BasketEntity();
        basket.AddItem(product.Id, product.UnitPrice, 1);
        var basketId = await CreateBasketAsync(basket, context);
        var dto = new BasketItemDto{ BasketId = basketId, ProductId = product.Id, Quantity = 2 };

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.BasketItems.Should().ContainSingle(i => i.ProductId == product.Id && i.Quantity == 3);
    }

    [Fact]
    public async Task AddItemAsync_ShouldFail_WhenUnableToSaveChanges()
    {
        // Arrange
        using var context = CreateContext();
        var ctxMock = new Mock<DataContext>(Options) { CallBase = true };
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object);
        var dto = new Mock<BasketItemDto>().Object;

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error?.Message.Contains("Item could not be added");
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldRemoveItem_WhenExists()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var product = await CreateProductAsync();
        var basket = new BasketEntity();
        basket.AddItem(product.Id, product.UnitPrice, 2);
        var basketId = await CreateBasketAsync(basket, context);
        var dto = new BasketItemDto{ BasketId = basketId, ProductId = product.Id, Quantity = 0 };

        // Act
        var result = await service.RemoveItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Type.Should().Be(ResultTypeEnum.Accepted);
        var after = await context.Baskets.Include(b => b.BasketItems).SingleAsync(b => b.Id == basketId);
        after.BasketItems.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldFail_WhenBasketMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var dto = new BasketItemDto{ BasketId = 424242, ProductId = 1 };

        // Act
        var result = await service.RemoveItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error!.Message.Should().Contain("Basket not found");
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldFail_WhenUnableToSaveChanges()
    {
        // Arrange
        using var context = CreateContext();
        var ctxMock = new Mock<DataContext>(Options) { CallBase = true };
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object);
        var product = await CreateProductAsync();
        var basket = new BasketEntity();
        basket.AddItem(product.Id, basket.Id, 1);
        var basketId = await CreateBasketAsync(basket, context);
        var dto = new BasketItemDto { BasketId = basket.Id, ProductId = product.Id };

        // Act
        var result = await service.RemoveItemAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error?.Message.Contains("Item could not be removed");
    }

    [Fact]
    public async Task AddCouponAsync_ShouldFail_WhenEmptyCode()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basketId = await CreateBasketAsync(new BasketEntity ("cs_any", "pi_any"), context);

        // Act
        var res = await service.AddCouponAsync(new BasketCouponDto{ BasketId = basketId, Code = "" });

        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Type.Should().Be(ResultTypeEnum.Invalid);
        res.Error!.Message.Should().Contain("Coupon code is required");
    }

    [Fact]
    public async Task AddCouponAsync_ShouldFail_WhenBasketMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);

        // Act
        var res = await service.AddCouponAsync(new BasketCouponDto { BasketId = 999, Code = "SAVE10" });
        
        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Type.Should().Be(ResultTypeEnum.Invalid);
        res.Error!.Message.Should().Contain("Unable to apply voucher");
    }

    [Fact]
    public async Task AddCouponAsync_ShouldFail_WhenNoClientSecret()
    {
        // Arrange
        using var context = CreateContext();
        var basketId = await CreateBasketAsync(new BasketEntity(), context);
        var service = Service(context);

        // Act
        var result = await service.AddCouponAsync(new BasketCouponDto { BasketId = basketId, Code = "SAVE10" });
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Unable to apply voucher");
    }

    [Fact]
    public async Task AddCouponAsync_ShouldFail_WhenInValidPayment()
    {
        // Arrange
        using var context = CreateContext();
        var product = await CreateProductAsync();
        var basket = new BasketEntity("cs_existing", "pi_existing");
        basket.AddItem(product.Id, product.UnitPrice, 2);
        var basketId = await CreateBasketAsync(basket, context);
        var dto = new BasketCouponDto { BasketId = basketId, Code = "SAVE10" };

        var paymentGateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        paymentGateway.Setup(x => x.TryGetCouponByPromoCodeAsync(dto.Code)).ReturnsAsync((CouponInfoModel?)null);
        var service = Service(context, paymentGateway);

        // Act
        var result = await service.AddCouponAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Unable to apply coupon");
    }

    [Fact]
    public async Task AddCouponAsync_ShouldFail_WhenInvalidPaymentIntentCreation()
    {
        // Arrange
        using var context = CreateContext();
        var product = await CreateProductAsync();
        var basket = new BasketEntity("cs_existing", "pi_existing");
        basket.AddItem(product.Id, product.UnitPrice, 2);
        var basketId = await CreateBasketAsync(basket, context);
        var basketCouponDto = new BasketCouponDto { BasketId = basketId, Code = "SAVE10" };
        var couponInfo = new CouponInfoModel("c_123", "Save Ten", "promoCodeId", "SAVE10", 1000, null);
        var piModel = new PaymentIntentModel("pi_123", "cs_123");

        var paymentGateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        paymentGateway.Setup(x => x.TryGetCouponByPromoCodeAsync(basketCouponDto.Code)).ReturnsAsync(couponInfo);
        paymentGateway.Setup(x => x.CreateOrUpdateAsync(It.IsAny<long>(), "aud", It.IsAny<string?>())).ReturnsAsync(piModel);

        var ctxMock = CreateMockContext();
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object, paymentGateway);

        // Act
        var result = await service.AddCouponAsync(new BasketCouponDto { BasketId = basketId, Code = "SAVE10"});

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error?.Message.Should().Contain("Coupon could not be added");
        paymentGateway.VerifyAll();
        ctxMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCouponAsync_ShouldApplyCouponAndUpdatePaymentIntent_WhenValid()
    {
        // Arrange
        using var context = CreateContext();
        var product = await CreateProductAsync();
        var basket = new BasketEntity("cs_existing", "pi_existing");
        basket.AddItem(product.Id, product.UnitPrice, 2);
        var basketId = await CreateBasketAsync(basket, context);
        var basketCouponDto = new BasketCouponDto { BasketId = basketId, Code = "SAVE10" };
        var couponInfo = new CouponInfoModel("c_123", "Save Ten", "promoCodeId", "SAVE10", 1000, null);
        var piModel = new PaymentIntentModel("pi_123", "cs_123");

        var paymentGateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        paymentGateway.Setup(x => x.TryGetCouponByPromoCodeAsync(basketCouponDto.Code)).ReturnsAsync(couponInfo);
        paymentGateway.Setup(x => x.CreateOrUpdateAsync(It.IsAny<long>(), "aud", It.IsAny<string?>())).ReturnsAsync(piModel);
        var service = Service(context, paymentGateway);

        // Act
        var result = await service.AddCouponAsync(new BasketCouponDto { BasketId = basketId, Code = "SAVE10" });

        // Assert
        paymentGateway.VerifyAll();
        result.IsSuccess.Should().BeTrue();
        result.Type.Should().Be(ResultTypeEnum.Success);
        result.Value!.Coupon.Should().NotBeNull();
        result.Value!.ClientSecret.Should().Be("cs_123");
        var after = await context.Baskets.Include(b => b.Coupon).SingleAsync(b => b.Id == basketId);
        after.Coupon.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveCouponAsync_ShouldRemove_WhenPresent()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basket = new BasketEntity ("cs_existing", "pi_existing");
        basket.AddCoupon(CouponEntity.CreateOnAmountOff("Any", "cup_1", "ANY", 10m));
        var basketId = await CreateBasketAsync(basket, context);
        var basketItemDto = new BasketCouponDto{ BasketId = basketId, Code = "ANY" };

        // Act
        var result = await service.RemoveCouponAsync(basketItemDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Type.Should().Be(ResultTypeEnum.Accepted);
        var after = await context.Baskets.Include(b => b.Coupon).SingleAsync(b => b.Id == basketId);
        after.Coupon.Should().BeNull();
    }

    [Fact]
    public async Task RemoveCouponAsync_ShouldFail_WhenNoCouponOnBasket()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basketId = await CreateBasketAsync(new BasketEntity ("cs_existing", "pi_existing"), context);
        var basketCouponDto = new BasketCouponDto { BasketId = basketId, Code = "ANY" };

        // Act
        var result = await service.RemoveCouponAsync(basketCouponDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Unable to update basket with coupon");
    }

    [Fact]
    public async Task RemoveCouponAsync_ShouldFail_WhenBasketMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = Service(context);
        var basketCouponDto = new BasketCouponDto { BasketId = 9999, Code = "ANY" };

        // Act
        var result = await service.RemoveCouponAsync(basketCouponDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Unable to update basket with coupon");
    }

    [Fact]
    public async Task RemoveCouponAsync_ShouldFail_WhenUnableToSaveChanges()
    {
        // Arrange
        using var context = CreateContext();
        var basket = new BasketEntity("cs_existing", "pi_existing");
        var coupon = new CouponEntity("Any", "cup_1", "ANY", 10m);
        basket.AddCoupon(coupon);
        var basketId = await CreateBasketAsync(basket, context);
        var basketCouponDto = new BasketCouponDto { BasketId = basket.Id, Code = "ANY" };

        var ctxMock = CreateMockContext();
        ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var service = Service(ctxMock.Object);

        // Act
        var result = await service.RemoveCouponAsync(basketCouponDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.Invalid);
        result.Error!.Message.Should().Contain("Coupon could not be removed");
    }
}