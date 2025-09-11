namespace BackendTests.UnitTests.Domain.Entities;

public class OrderEntityUnitTests 
{
    private static OrderEntity NewOrder(int id = 0, string email = "USER@EXAMPLE.COM", decimal delivery = 5m, decimal subtotal = 20m,decimal discount = 0m, List<OrderItemEntity>? items = null)
    {
        items ??= new List<OrderItemEntity> { new OrderItemEntity(1, "P", 10m, 2) };
        var address = new ShippingAddress("12 Main", null, "Sydney", "NSW", "2000", "AU");
        return new OrderEntity(email, address, "pi_123", delivery, subtotal, discount, new(), items);
    }

    [Fact]
    public void Constructor_ShouldWireCoreFields_WhenInputIsValid()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var items = new List<OrderItemEntity> { new OrderItemEntity(1, "P", 10m, 2) };
        var address = new ShippingAddress("12 Main", null, "Sydney", "NSW", "2000", "AU");

        // Act
        var order = new OrderEntity("USER@X.COM", address, "pi_123", 5m, 20m, 2m, new(), items);

        // Assert
        order.UserEmail.Should().Be("user@x.com");
        order.PaymentIntentId.Should().Be("pi_123");
        order.ShippingAddress.Should().NotBeNull();
        order.PaymentSummary.Should().NotBeNull();
        order.OrderItems.Should().HaveCount(1);
        order.Subtotal.Should().Be(20m);
        order.DeliveryFee.Should().Be(5m);
        order.Discount.Should().Be(2m);
        order.Total.Should().Be(23m); // 20 + 5 - 2
        order.OrderStatusId.Should().Be((int)OrderStatusEnum.Pending);
        order.OrderDate.Should().BeOnOrAfter(now);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetUserEmail_ShouldThrowError_WhenInputIsMissing(string? userEmail)
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        Action act = () => entity.SetUserEmail(userEmail!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(userEmail)} is ivalid.*");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(99)]
    public void SetUserEmail_ShouldThrowError_WhenInputIsTooLong(int length)
    {
        // Arrange
        var entity = NewOrder();
        var userEmail = new string('a', length);
        
        // Act
        Action act = () => entity.SetUserEmail(userEmail);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(userEmail)} is too long.*");
    }

    [Theory]
    [InlineData("   test@nwc.so  ", "test@nwc.so")]
    [InlineData("A@B.COM   ", "a@b.com")]
    [InlineData("    ElsO@mL.coM ", "elso@ml.com")]
    public void SetUserEmail_ShouldLowercaseAndTrimEmail_WhenInputIsValid(string userEmail, string expected)
    {
        // Arrange
        var entity = NewOrder(10, email: "X");

        // Act
        entity.SetUserEmail(userEmail);

        // Assert
        entity.UserEmail.Should().Be(expected);
    }

    [Fact]
    public void SetShippingAddress_ShouldThrowError_WhenInputIsNull()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        Action act = () => entity.SetShippingAddress(null!);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*address is required.*");
    }

    [Fact]
    public void SetPaymentSummary_ShouldThrowError_WhenInputIsNull()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        Action act = () => entity.SetPaymentSummary(null!);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*summary is required.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetPaymentIntent_ShouldThrowError_WhenInputIsMissing(string? paymentIntentId)
    {
        // Assert
        var entity = NewOrder();
        
        // Act
        Action act = () => entity.SetPaymentIntent(paymentIntentId);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(paymentIntentId)} is required.*");
    }

    [Theory]
    [InlineData(-99)]
    [InlineData(-1)]
    [InlineData(0)]
    public void AddItem_ShouldThrowError_WhenQuantityIsNonPositive(int quantity)
    {
        // Arrange
        var entity = NewOrder(items: new());
        
        // Act
        Action act = () => entity.AddItem(1, "x", 10m, quantity);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Theory]
    [InlineData(-99.9)]
    [InlineData(-1.0001)]
    [InlineData(-0.000009)]
    public void AddItem_ShouldThrowError_WhenUnitPriceIsNonPositive(decimal unitPrice)
    {
        // Arrange
        var entity = NewOrder(items: new());

        // Act
        Action act = () => entity.AddItem(1, "x", unitPrice, 10);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(unitPrice)} cannot be negative.*");
    }

    [Fact]
    public void AddItem_ShouldAddNewItem_WhenItemDoesNotExist()
    {
        // Arrange
        var entity = NewOrder(delivery: 0m, subtotal: 0m, discount: 0m, items: new ());

        // Act
        entity.AddItem(1, "A", 10m, 2);
        
        // Assert
        entity.Subtotal.Should().Be(20m);
        entity.OrderItems.Should().HaveCount(1);
    }

    [Fact]
    public void AddItem_ShouldMergeValues_WhenItemExists()
    {
        // Arrange
        var entity = NewOrder(delivery: 0m, subtotal: 0m, discount: 0m, items: new());
       
        // Act
        entity.AddItem(1, "x", 10m, 3);
        entity.AddItem(1, "x", 10m, 5);

        // Assert
        entity.Subtotal.Should().Be(80m);
        entity.OrderItems.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(-9)]
    [InlineData(-1)]
    [InlineData(0)]
    public void RemoveItem_ShouldThrowError_WhenQuantityIsNonPositive(int quantity)
    {
        // Arrange
        var entity = NewOrder(items: new());
        entity.AddItem(1, "x", 10m, 2);

        // Act
        Action act = () => entity.RemoveItem(1, quantity);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void RemoveItem_ShouldThrowError_WhenItemIsNotFound()
    {
        // Arrange
        var entity = NewOrder(items: new());
        entity.AddItem(1, "x", 10m, 2);

        // Act
        Action act = () => entity.RemoveItem(999, 1);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Item not found.*");
    }

    [Fact]
    public void RemoveItem_ShouldDecreaseQuantity_WhenInputsAreValid()
    {
        // Arrange
        var entity = NewOrder(delivery: 0m, subtotal: 0m, discount: 0m, items: new());
        entity.AddItem(1, "x", 10m, 3); // 30

        // Act
        entity.RemoveItem(1, 2);
        
        // Assert
        entity.Subtotal.Should().Be(10m);
        entity.OrderItems.Single().Quantity.Should().Be(1);
    }

    [Fact]
    public void RemoveItem_ShouldDecreaseAndRemoveAtZero_WhenInputsAreValid()
    {
        // Arrange
        var entity = NewOrder(delivery: 0m, subtotal: 0m, discount: 0m, items: new());
        entity.AddItem(1, "x", 10m, 3);

        // Act
        entity.RemoveItem(1, 2);    
        entity.RemoveItem(1, 1);

        // Assert
        entity.Subtotal.Should().Be(0m);
        entity.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void UpdateCharges_ShouldDelegatesToSetters_WhenCalled()
    {
        // Arrange
        var entity = NewOrder();

        // Act
        entity.UpdateCharges(deliveryFee: 12m, subtotal: 50m, discount: 5m);

        // Assert
        entity.DeliveryFee.Should().Be(12m);
        entity.Subtotal.Should().Be(50m);
        entity.Discount.Should().Be(5m);
        entity.Total.Should().Be(57m);
    }

    [Theory]
    [InlineData(5, 20)]
    [InlineData(92.3, 120.2)]
    public void SetDeliveryFee_ShouldSetFee_WhenInputIsValid(decimal deliveryFee, decimal subtotal)
    {
        // Arrange
        var entity = NewOrder(delivery: deliveryFee, subtotal: subtotal, discount: 0m);
        var expectedTotal = subtotal + deliveryFee;

        // Act
        entity.SetDeliveryFee(deliveryFee);

        // Assert
        entity.DeliveryFee.Should().Be(deliveryFee);
        entity.Total.Should().Be(expectedTotal);
    }

    [Theory]
    [InlineData(-7.0)]
    [InlineData(-0.2)]
    [InlineData(-5.5)]
    public void SetDeliveryFee_ShouldThrowError_WhenInputIsInValid(decimal amount)
    {
        // Arrange
        var entity = NewOrder();

        // Act
        Action act = () => entity.SetDeliveryFee(amount);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"{nameof(amount)} cannot be negative.");
    }

    [Theory]
    [InlineData(5, 20)]
    [InlineData(92.3, 120.2)]
    public void SetSubtotal_ShouldSetSubtotal_WhenInputIsValid(decimal deliveryFee, decimal subtotal)
    {
        // Arrange
        var entity = NewOrder(delivery: deliveryFee, subtotal: subtotal, discount: 0m);
        var expectedTotal = subtotal + deliveryFee;

        // Act
        entity.SetSubtotal(subtotal);

        // Assert
        entity.Subtotal.Should().Be(subtotal);
        entity.Total.Should().Be(expectedTotal);
    }

    [Theory]
    [InlineData(-7.0)]
    [InlineData(-0.2)]
    [InlineData(-5.5)]
    public void SetSubtotal_ShouldThrowError_WhenInputIsInValid(decimal amount)
    {
        // Arrange
        var entity = NewOrder();

        // Act
        Action act = () => entity.SetSubtotal(amount);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"{nameof(amount)} cannot be negative.");
    }

    [Theory]
    [InlineData(-7.0)]
    [InlineData(-0.2)]
    [InlineData(-5.5)]
    public void ApplyDiscount_ShouldThrowError_WhenInputIsInValid(decimal amount)
    {
        // Arrange
        var entity = NewOrder();

        // Act
        Action act = () => entity.ApplyDiscount(amount);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"{nameof(amount)} cannot be negative.");
    }

    [Fact]
    public void ApplyDiscount_ShouldThrowError_WhenInputIsGreaterThanAmount()
    {
        // Arrange
        var entity = NewOrder(subtotal: 100m, delivery: 50m);

        // Act
        Action act = () => entity.ApplyDiscount(200);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"amount exceeds order amount.");
    }

    [Fact]
    public void ApplyDiscount_ShouldSetDiscount_WhenInputIsValid()
    {
        // Arrange
        var entity = NewOrder(subtotal: 100m, delivery: 50m);

        // Act
        entity.ApplyDiscount(50);

        // Assert
        entity.Discount.Should().Be(50);
    }

    [Fact]
    public void ClearDiscount_ShouldResetDiscount_WhenCalled()
    {
        // Arrange
        var entity = NewOrder(subtotal: 100m, delivery: 50m, discount: 100m);

        // Act
        entity.ClearDiscount();

        // Assert
        entity.Discount.Should().Be(0);
    }

    [Fact]
    public void MarkPaymentSucceeded_ShouldSetPaidAndStoresStripeEventId_WhenInputsAreValid()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        entity.MarkPaymentSucceeded("evt_1");

        // Assert
        entity.OrderStatusId.Should().Be((int)OrderStatusEnum.Paid);
        entity.LastProcessedStripeEventId.Should().Be("evt_1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MarkPaymentSucceeded_ShouldThrowError_WhenInputIsMissing(string? stripeEventId)
    {
        // Arrange
        var entity = NewOrder();

        // Act
        Action act = () => entity.MarkPaymentSucceeded(stripeEventId!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(stripeEventId)} cannot be empty.*");
    }

    [Fact]
    public void MarkPaymentSucceeded_ShouldDoNothing_WhenStripeIdIsTheSame()
    {
        // Arrange
        var entity = NewOrder();
        var eventId = "evt_1";
        var beforeStatus = entity.OrderStatusId;

        // Act
        entity.MarkPaymentSucceeded(eventId);
        var afterUpdate = entity.OrderStatusId;
        entity.MarkPaymentSucceeded(eventId);

        // Assert
        entity.OrderStatusId.Should().NotBe(beforeStatus);
        entity.OrderStatusId.Should().Be(afterUpdate);
    }

    [Fact]
    public void MarkCancelled_ShouldCancelOrder_WhenCalled()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        entity.MarkCancelled();
        
        // Assert
        entity.OrderStatusId.Should().Be((int)OrderStatusEnum.Cancelled);
    }

    [Fact]
    public void MarkShipped_ShouldChangeStatus_WhenCalled()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        entity.MarkPaymentSucceeded("evt_1");
        entity.MarkShipped();
        
        // Assert
        entity.OrderStatusId.Should().Be((int)OrderStatusEnum.Shipped);
    }

    [Fact]
    public void MarkCompleted_ShouldChangeFromShipped_Transitions()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        entity.MarkPaymentSucceeded("evt_1");
        entity.MarkShipped();
        entity.MarkCompleted();

        // Assert
        entity.OrderStatusId.Should().Be((int)OrderStatusEnum.Completed);
    }

    [Fact]
    public void MarkPaymentFailed_ShouldChangeFromPending_WhenCalled()
    {
        // Arrange
        var entity = NewOrder();

        // Act
        entity.MarkPaymentFailed();
        
        // Assert
        entity.OrderStatusId.Should().Be((int)OrderStatusEnum.PaymentFailed);
    }

    [Fact]
    public void IllegalTransition_ShouldThrowError_WhenCalled()
    {
        // Arrange
        var entity = NewOrder();
        
        // Act
        Action act = () => entity.MarkCompleted();
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Illegal transition*");
    }

    [Fact]
    public void RecalculateSubtotal_ShouldUpdateTotal_WhenDiscountIsApplied()
    {
        // Arrange
        var entity = NewOrder(delivery: 5m, subtotal: 0m, discount: 0m, items: new ());
        entity.AddItem(1, "A", 10m, 3);
        entity.AddItem(2, "B", 20m, 1);

        // Act
        entity.ApplyDiscount(30m);
        entity.AddItem(1, "C", 30m, 1);
        entity.AddItem(2, "D", 10m, 1);
        entity.ApplyDiscount(30m);

        // Assert
        entity.Discount.Should().Be(30m);
        entity.Total.Should().Be(55m);
    }

    [Fact]
    public void RecalculateSubtotal_ShouldUpdateTotal_WhenDeliveryIsApplied()
    {
        // Arrange
        var entity = NewOrder(delivery: 0m, subtotal: 100m, discount: 0m, items: new());
        entity.AddItem(1, "A", 10m, 3);
        entity.AddItem(2, "B", 20m, 1);

        // Act
        entity.ApplyDiscount(15m);
        entity.AddItem(1, "C", 30m, 1);
        entity.AddItem(2, "D", 10m, 1);
        entity.ApplyDiscount(30m);
        entity.SetDeliveryFee(15m);

        // Assert
        entity.Discount.Should().Be(30m);
        entity.Total.Should().Be(65m);
    }
}
