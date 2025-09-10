namespace BackendTests.UnitTests.Domain.Entities;

public class BasketItemEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetFieldsAndConductComputations_WhenInstantiated()
    {
        // Arrange
        var product = new ProductEntity("P", "D", 10m, 100, 1, 1, null);

        // Act
        var item = new BasketItemEntity(10, 20m, 3, product);

        // Assert
        item.ProductId.Should().Be(10);
        item.UnitPrice.Should().Be(20m);
        item.Quantity.Should().Be(3);
        item.Product.Should().BeSameAs(product);
        item.LineTotal.Should().Be(60m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public void Constructor_ShouldThrowError_WhenProductIdIsInvalid(int productId)
    {
        // Act
        Action act = () => new BasketItemEntity(productId, 10m, 1, null);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(productId)} cannot be less than 0.*");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void Constructor_ShouldThrowError_WhenUnitPriceIsNegavitve(decimal unitPrice)
    {
        // Act
        Action act = () => new BasketItemEntity(1, unitPrice, 1, null);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(unitPrice)} cannot be negative.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_ShouldThrowError_WhenQuantityIsInvalid(int quantity)
    {
        // Act
        Action act = () => new BasketItemEntity(1, 10m, quantity, null);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void IncreaseQuantity_ShouldIncreaseQuantity_WhenCalled()
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        item.IncreaseQuantity(3);

        // Assert
        item.Quantity.Should().Be(5);
        item.LineTotal.Should().Be(50m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_ShouldThrowError_WhenInputIsNonPositive(int quantity)
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        Action act = () => item.IncreaseQuantity(quantity);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void IncreaseQuantity_ShouldThrowError_WhenPropertyOverflows()
    {
        // Arrange
        var start = int.MaxValue - 1;
        var item = new BasketItemEntity(1, 1m, start);

        // Act
        Action act = () => item.IncreaseQuantity(2);

        // Assert
        act.Should().Throw<OverflowException>();
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(99, 52)]
    [InlineData(1, 1)]
    public void DecreaseQuantity_ShouldDecreaseQuantity_WhenInputIsPositive(int initQty, int quantity)
    {
        // Arrange
        var unitPrice = 10m;
        var finalExpectedQty = initQty - quantity;
        var lineTotal = finalExpectedQty * unitPrice;
        var item = new BasketItemEntity(1, unitPrice, initQty);

        // Acct
        item.DecreaseQuantity(quantity);
        
        // Assert
        item.Quantity.Should().Be(finalExpectedQty);
        item.LineTotal.Should().Be(lineTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseQuantity_ShouldThrowError_WhenQuantityIsNonPositive(int quantity)
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        Action act = () => item.DecreaseQuantity(quantity);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*Quantity must be positive.*");
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowError_WhenReducingBelowZero()
    {
        // Arrange
        var quantity = 3;
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        Action act = () => item.DecreaseQuantity(quantity);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*Cannot reduce {nameof(quantity)} below zero.*");
    }

    [Fact]
    public void ReplaceQuantity_ShouldReplaceQuantity_WhenInputIsPositive()
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        item.ReplaceQuantity(7);

        // Assert
        item.Quantity.Should().Be(7);
        item.LineTotal.Should().Be(70m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ReplaceQuantity_ShouldThrowError_WhenInputIsNonPositive(int quantity)
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        Action act = () => item.ReplaceQuantity(quantity);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Theory]
    [InlineData(2, 10.0)]
    [InlineData(5, 5.50)]
    public void SetUnitPrice_ShouldSetPrice_WhenInputIsNonNegative(int quantity, decimal unitPrice)
    {
        // Arrange
        var item = new BasketItemEntity(1, unitPrice, quantity);
        var lineTotal = quantity * unitPrice;

        // Act
        item.SetUnitPrice(unitPrice);
        
        // Assert
        item.UnitPrice.Should().Be(unitPrice);
        item.LineTotal.Should().Be(lineTotal);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void SetUnitPrice_ShouldThrowError_WhenInputIsNegative(decimal unitPrice)
    {
        // Arrange
        var item = new BasketItemEntity(1, 10m, 2);
        
        // Act
        Action act = () => item.SetUnitPrice(unitPrice);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{nameof(unitPrice)} cannot be negative.*");
    }
}
