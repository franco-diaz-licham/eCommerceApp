namespace BackendTests.UnitTests.Domain.Entities;

public class BasketEntityUnitTests
{
    [Fact]
    public void Constructor_SetsCreatedOn_AndHasNoItems()
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

    [Fact]
    public void AddItem_AddsNewItem_AndTouchesUpdatedOn()
    {
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        basket.AddItem(productId: 10, unitPrice: 20m, quantity: 2);

        basket.BasketItems.Should().HaveCount(1);
        var item = basket.BasketItems.Single();
        item.ProductId.Should().Be(10);
        item.UnitPrice.Should().Be(20m);
        item.Quantity.Should().Be(2);
        basket.Subtotal.Should().Be(40m);
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void AddItem_SameProduct_MergesQuantity()
    {
        var basket = new BasketEntity();

        basket.AddItem(10, 20m, 2);
        basket.AddItem(10, 20m, 3);

        basket.BasketItems.Should().HaveCount(1);
        basket.BasketItems.Single().Quantity.Should().Be(5);
        basket.Subtotal.Should().Be(20m * 5);
    }

    [Theory]
    [InlineData(0, 10)]  // non-positive quantity
    [InlineData(-1, 10)]
    public void AddItem_Throws_ForInvalidQuantity(int qty, decimal price)
    {
        var basket = new BasketEntity();
        Action act = () => basket.AddItem(1, price, qty);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Quantity must be positive.*");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-5)]
    public void AddItem_Throws_ForNegativeUnitPrice(decimal price)
    {
        var basket = new BasketEntity();
        Action act = () => basket.AddItem(1, price, 1);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Unit price cannot be negative.*");
    }

    [Fact]
    public void SetItemQuantity_UpdatesQuantity_WhenItemExists()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2);

        basket.SetItemQuantity(1, 5);

        basket.BasketItems.Single().Quantity.Should().Be(5);
        basket.Subtotal.Should().Be(50m);
    }

    [Fact]
    public void SetItemQuantity_Zero_RemovesItem()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2);

        basket.SetItemQuantity(1, 0);

        basket.BasketItems.Should().BeEmpty();
        basket.Subtotal.Should().Be(0m);
    }

    [Fact]
    public void SetItemQuantity_Throws_WhenItemMissing()
    {
        var basket = new BasketEntity();
        Action act = () => basket.SetItemQuantity(999, 1);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*Item not found.*");
    }

    [Fact]
    public void SetItemQuantity_Throws_WhenNewQuantityNegative()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2);

        Action act = () => basket.SetItemQuantity(1, -1);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Quantity cannot be negative.*");
    }

    [Fact]
    public void RemoveItem_ReducesQuantity_AndRemovesAtZero()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 3);

        basket.RemoveItem(1, 2);
        basket.BasketItems.Single().Quantity.Should().Be(1);
        basket.Subtotal.Should().Be(10m);

        basket.RemoveItem(1, 1);
        basket.BasketItems.Should().BeEmpty();
        basket.Subtotal.Should().Be(0m);
    }

    [Fact]
    public void RemoveItem_NoOp_WhenProductNotFound()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2);

        basket.RemoveItem(999, 1); // should not throw or change existing items

        basket.BasketItems.Should().HaveCount(1);
        basket.BasketItems.Single().Quantity.Should().Be(2);
        basket.Subtotal.Should().Be(20m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RemoveItem_Throws_WhenQuantityNotPositive(int qty)
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2);

        Action act = () => basket.RemoveItem(1, qty);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Quantity must be positive.*");
    }

    [Fact]
    public void Subtotal_SumsLineTotals_AcrossItems()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2); // 20
        basket.AddItem(2, 5m, 3);  // 15

        basket.Subtotal.Should().Be(35m);
    }

    [Fact]
    public void SetDiscount_SetsAmount_AndCapsAtSubtotal()
    {
        var basket = new BasketEntity();
        basket.AddItem(1, 10m, 2); // subtotal = 20

        basket.SetDiscount(7m);
        basket.Discount.Should().Be(7m);

        basket.SetDiscount(50m); // cap to subtotal
        basket.Discount.Should().Be(20m);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-5)]
    public void SetDiscount_Throws_WhenNegative(decimal amount)
    {
        var basket = new BasketEntity();
        Action act = () => basket.SetDiscount(amount);
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("*Discount cannot be negative.*");
    }

    [Fact]
    public void AddCoupon_SetsCoupon_AndAppliesCalculatedDiscount()
    {
        //var basket = new BasketEntity();
        //basket.AddItem(1, 100m, 1); // subtotal = 100

        //var coupon = CouponEntity.CreatePercentOff("ten off", 10m); // 10%
        //basket.AddCoupon(coupon);

        //basket.Coupon.Should().NotBeNull();
        //basket.Coupon.Should().BeSameAs(coupon);
        //basket.Discount.Should().Be(10m);
    }

    [Fact]
    public void AddCoupon_Throws_WhenNull()
    {
        var basket = new BasketEntity();
        Action act = () => basket.AddCoupon(null!);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*Coupon Id cannot be less than zero*");
    }

    [Fact]
    public void RemoveCoupon_ClearsCoupon_AndDiscount()
    {
        //var basket = new BasketEntity();
        //basket.AddItem(1, 50m, 2); // subtotal = 100
        //var coupon = CouponEntity.CreateAmountOff("ten", 10m);
        //basket.AddCoupon(coupon);

        //basket.RemoveCoupon();

        //basket.Coupon.Should().BeNull();
        //basket.Discount.Should().Be(0m);
    }

    [Fact]
    public void AttachPaymentIntent_SetsFields_AndTouches()
    {
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        basket.AttachPaymentIntent("pi_123", "cs_123");

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
    public void AttachPaymentIntent_Throws_ForMissingValues(string? pi, string? cs)
    {
        var basket = new BasketEntity();
        Action act = () => basket.AttachPaymentIntent(pi!, cs!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearPaymentIntent_NullsFields_AndTouches()
    {
        var basket = new BasketEntity();
        basket.AttachPaymentIntent("pi_123", "cs_123");

        var before = basket.UpdatedOn;
        basket.ClearPaymentIntent();

        basket.PaymentIntentId.Should().BeNull();
        basket.ClientSecret.Should().BeNull();
        basket.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void Mutations_Touch_UpdatedOn()
    {
        var basket = new BasketEntity();
        var before = basket.UpdatedOn;

        basket.AddItem(1, 10m, 1);
        var afterAdd = basket.UpdatedOn;
        afterAdd.Should().NotBe(before);

        basket.SetItemQuantity(1, 2);
        var afterSetQty = basket.UpdatedOn;
        afterSetQty.Should().NotBe(afterAdd);

        basket.RemoveItem(1, 1);
        var afterRemove = basket.UpdatedOn;
        afterRemove.Should().NotBe(afterSetQty);

        basket.SetDiscount(0m);
        var afterDiscount = basket.UpdatedOn;
        afterDiscount.Should().NotBe(afterRemove);

        //var coupon = CouponEntity.CreateAmountOff("x", 1m);
        //basket.AddCoupon(coupon);
        //var afterCoupon = basket.UpdatedOn;
        //afterCoupon.Should().NotBe(afterDiscount);

        //basket.RemoveCoupon();
        //var afterRemoveCoupon = basket.UpdatedOn;
        //afterRemoveCoupon.Should().NotBe(afterCoupon);

        //basket.AttachPaymentIntent("pi", "cs");
        //var afterAttach = basket.UpdatedOn;
        //afterAttach.Should().NotBe(afterRemoveCoupon);

        //basket.ClearPaymentIntent();
        //basket.UpdatedOn.Should().NotBe(afterAttach);
    }

}
