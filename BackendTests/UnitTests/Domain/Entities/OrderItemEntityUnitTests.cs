namespace BackendTests.UnitTests.Domain.Entities;

public class OrderItemEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetsFields_WhenInstantiated()
    {
        // Arrange and Act
        var item = new OrderItemEntity(productId: 1, productName: "Widget", unitPrice: 10m, quantity: 3);

        // Assert
        item.ProductId.Should().Be(1);
        item.ProductName.Should().Be("Widget");
        item.UnitPrice.Should().Be(10m);
        item.Quantity.Should().Be(3);
        item.LineTotal.Should().Be(30m);
        item.UpdatedOn.Should().BeNull();
    }

    [Theory]
    [InlineData(0, "Widget", 10, 1, "Invalid product.")]
    [InlineData(-1, "Widget", 10, 1, "Invalid product.")]
    [InlineData(1, "", 10, 1, "Product name required.")]
    [InlineData(1, " ", 10, 1, "Product name required.")]
    [InlineData(1, "Widget", -1, 1, "Unit price cannot be negative.")]
    [InlineData(1, "Widget", 10, 0, "Quantity must be positive.")]
    public void Constructor_ShouldThrowError_WhenInputsAreInvalid(int pid, string name, decimal price, int qty, string expectedMessage)
    {
        // Arrang and Act
        Action act = () => new OrderItemEntity(pid, name, price, qty);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{expectedMessage}*");
    }

    [Fact]
    public void IncreaseQuantity_ShouldAddToQuantity_WhenValidInput()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        item.IncreaseQuantity(3);

        // Assert
        item.Quantity.Should().Be(5);
        item.LineTotal.Should().Be(25m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_ShouldThrowError_WhenNonPositive(int quantity)
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        Action act = () => item.IncreaseQuantity(quantity);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void IncreaseQuantity_ShouldThrowError_WhenQuantityOverflows()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 1m, int.MaxValue);
        
        // Act
        Action act = () => item.IncreaseQuantity(1);
        
        // Assert
        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void DecreaseQuantity_ShouldSubtract_WhenInputIsValid()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 3);

        // Act
        item.DecreaseQuantity(2);

        // Assert
        item.Quantity.Should().Be(1);
        item.LineTotal.Should().Be(5m);
    }

    [Fact]
    public void DecreaseQuantity_ShouldSetToZero_WhenAllowed()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        item.DecreaseQuantity(2);


        // Assert
        item.Quantity.Should().Be(0);
        item.LineTotal.Should().Be(0m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseQuantity_ShouldThrowError_WhenNonPositive(int quantity)
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        Action act = () => item.DecreaseQuantity(quantity);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowErro_WhenReducingBelowZero()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        Action act = () => item.DecreaseQuantity(3);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*Cannot reduce quantity below zero.*");
    }

    [Fact]
    public void SetUnitPrice_ShouldSetWhenNonNegative_WhenInputIsValid()
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        item.SetUnitPrice(7.5m);

        // Assert
        item.UnitPrice.Should().Be(7.5m);
        item.LineTotal.Should().Be(15m);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void SetUnitPrice_ShouldThrowError_WhenInputIsNegative(int price)
    {
        // Arrange
        var item = new OrderItemEntity(1, "Widget", 5m, 2);

        // Act
        Action act = () => item.SetUnitPrice(price);


        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(price)} cannot be negative.*");
    }
}
