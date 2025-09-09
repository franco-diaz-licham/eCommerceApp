using Stripe;

namespace BackendTests.UnitTests.Domain.Entities;

public class BasketEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetCreatedOn_WhenInstantiated()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var basket = new BasketEntity(now);

        // Assert
        basket.CreatedOn.Should().Be(now);
        basket.UpdatedOn.Should().BeNull();
        basket.BasketItems.Should().BeEmpty();
        basket.Subtotal.Should().Be(0m);
        basket.Discount.Should().Be(0m);
    }

    [Theory]
    [InlineData(10, 20.0, 2)]
    [InlineData(11, 0.01, 1)]
    [InlineData(2, 1100.2, 111)]
    public void AddItem_ShouldAddNewItem_WhenInputIsValid(int productId, decimal unitPrice, int quantity)
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        // Act
        basket.AddItem(productId, unitPrice, quantity);
        var subtotal = unitPrice * quantity;

        // Assert
        basket.BasketItems.Should().HaveCount(1);
        var item = basket.BasketItems.Single();
        item.ProductId.Should().Be(productId);
        item.UnitPrice.Should().Be(unitPrice);
        item.Quantity.Should().Be(quantity);
        basket.Subtotal.Should().Be(subtotal);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void AddItem_ShouldMergeQuantity_WhenInputIsSameProduct()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        // Act
        basket.AddItem(10, 20m, 2);
        basket.AddItem(10, 20m, 3);

        // Assert
        basket.BasketItems.Should().HaveCount(1);
        basket.BasketItems.Single().Quantity.Should().Be(5);
        basket.Subtotal.Should().Be(20m * 5);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(0, 10)] 
    [InlineData(-1, 10)]
    public void AddItem_ShouldThrowError_WhenQuantityIsInvalid(int quantity, decimal unitPrice)
    {
        // Arrange
        var basket = new BasketEntity();

        // Act
        Action act = () => basket.AddItem(1, unitPrice, quantity);

        // Arrange
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-5)]
    public void AddItem_ShouldThrowError_WhenUnitPriceIsInvalid(decimal unitPrice)
    {
        // Arrange
        var basket = new BasketEntity();
        
        // Act
        Action act = () => basket.AddItem(1, unitPrice, 1);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(unitPrice)} cannot be negative.*");
    }

    [Theory]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(999)]
    public void SetItemQuantity_ShouldUpdateQuantity_WhenItemExists(int quantity)
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        decimal unitPrice = 10m;
        int initQuantity = 2;

        // Act
        basket.AddItem(1, unitPrice, initQuantity);
        basket.SetItemQuantity(1, quantity);
        var subtotal = unitPrice * quantity;

        // Assert
        basket.BasketItems.Single().Quantity.Should().Be(quantity);
        basket.Subtotal.Should().Be(subtotal);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void SetItemQuantity_ShouldRemoveItem_WhenQuantityIsZero()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        // Act
        basket.AddItem(1, 10m, 2);
        basket.SetItemQuantity(1, 0);

        // Assert
        basket.BasketItems.Should().BeEmpty();
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(-99)]
    [InlineData(-10)]
    public void SetItemQuantity_ShouldThrowError_WhenQuantityIsInvalid(int quantity)
    {
        // Arrange
        var basket = new BasketEntity();

        // Act
        basket.AddItem(1, 10m, 2);
        Action act = () => basket.SetItemQuantity(1, quantity);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName($"{nameof(quantity)} cannot be negative.");
    }

    [Fact]
    public void SetItemQuantity_ShouldThrowError_WhenItemIsMissing()
    {
        // Arrange
        var basket = new BasketEntity();
        
        // Act
        Action act = () => basket.SetItemQuantity(999, 1);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*item not found.*");
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenItemFound()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        basket.AddItem(1, 10m, 3);

        // Act
        basket.RemoveItem(1);
        
        // Assert
        basket.BasketItems.Should().BeEmpty();
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void RemoveItem_ShouldNoOpt_WhenItemIsMissing()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        basket.AddItem(1, 10m, 2);

        // Act
        basket.RemoveItem(10);

        // Assert
        basket.BasketItems.Should().HaveCount(1);
        basket.BasketItems.Single().Quantity.Should().Be(2);
        basket.Subtotal.Should().Be(20m);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(10.0)]
    [InlineData(50.2)]
    public void SetDiscount_ShouldSetAmount_WhenDiscountIsValid(decimal amount)
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        decimal unitPrice = 10m;
        int initQuantity = 2;
        var subtotal = unitPrice * initQuantity;
        basket.AddItem(1, unitPrice, initQuantity);

        // Act
        basket.SetDiscount(amount);

        // Assert
        if(subtotal < amount) basket.Discount.Should().Be(subtotal);
        else basket.Discount.Should().Be(amount);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-5)]
    public void SetDiscount_ShouldThrowError_WhenInputIsNegative(decimal amount)
    {
        // Arrange
        var basket = new BasketEntity();
        
        // Act
        Action act = () => basket.SetDiscount(amount);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(amount)} cannot be negative.*");
    }

    [Fact]
    public void AddCoupon_ShouldSetCouponAndAppliesCalculatedDiscount_WhenInputIsValid()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        basket.AddItem(1, 100m, 1);
        var coupon = CouponEntity.CreatePercentOff("ten off", "remoteId", "10%OFF", 10m);
        
        // Act
        basket.AddCoupon(coupon);

        // Assert
        basket.Coupon.Should().NotBeNull();
        basket.Coupon.Should().BeSameAs(coupon);
        basket.Discount.Should().Be(10m);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void AddCoupon_ShouldThrowError_WhenCouponIsNull()
    {
        // Arrange
        var basket = new BasketEntity();
        CouponEntity? coupon = null;

        // Act
        Action act = () => basket.AddCoupon(coupon!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(coupon)} cannot be less than zero*");
    }

    [Fact]
    public void RemoveCoupon_ShouldClearCouponAndDiscount_WhenCalled()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        basket.AddItem(1, 50m, 2);
        var coupon = CouponEntity.CreateAmountOff("ten off", "remoteId", "10%OFF", 10m);
        basket.AddCoupon(coupon);

        // Act
        basket.RemoveCoupon();

        // Assert
        basket.CouponId.Should().BeNull();
        basket.Coupon.Should().BeNull();
        basket.Discount.Should().Be(0m);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void AttachPaymentIntent_SetsFields_WhenCalled()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        // Act
        basket.AttachPaymentIntent("pi_123", "cs_123");

        // Assert
        basket.PaymentIntentId.Should().Be("pi_123");
        basket.ClientSecret.Should().Be("cs_123");
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(null, "cs")]
    [InlineData("", "cs")]
    [InlineData("   ", "cs")]
    [InlineData("pi", null)]
    [InlineData("pi", "")]
    [InlineData("pi", "   ")]
    public void AttachPaymentIntent_ShouldThrowError_ForMissingValues(string? pi, string? cs)
    {
        // Arrange
        var basket = new BasketEntity();
        
        // Act
        Action act = () => basket.AttachPaymentIntent(pi!, cs!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearPaymentIntent_ShouldNullFields_WhenCalled()
    {
        // Arrange
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;
        basket.AttachPaymentIntent("pi_123", "cs_123");

        // Act
        basket.ClearPaymentIntent();

        // Assert
        basket.PaymentIntentId.Should().BeNull();
        basket.ClientSecret.Should().BeNull();
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void Subtotal_ShouldSumLineTotals_WhenCalled()
    {
        // Arrange
        var basket = new BasketEntity();
        
        // Act
        basket.AddItem(1, 10m, 2);
        basket.AddItem(2, 5m, 3);

        // Assert
        basket.Subtotal.Should().Be(35m);
    }
}
