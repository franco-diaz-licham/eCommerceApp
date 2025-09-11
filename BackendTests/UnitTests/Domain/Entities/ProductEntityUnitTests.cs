namespace BackendTests.UnitTests.Domain.Entities;

public class ProductEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetCoreFields_WhenInstantiated()
    {
        // Arrange and Act
        var p = new ProductEntity(
            name: "  Apple   iPhone  ",
            description: "  Latest   model ",
            unitPrice: 999.99m,
            quantityInStock: 5,
            productTypeId: 1,
            brandId: 2,
            photoId: 1
        );

        // Assert
        p.Name.Should().Be("Apple iPhone");
        p.NameNormalized.Should().Be("APPLE IPHONE");
        p.Description.Should().Be("Latest   model".Trim());
        p.UnitPrice.Should().Be(999.99m);
        p.QuantityInStock.Should().Be(5);
        p.ProductTypeId.Should().Be(1);
        p.BrandId.Should().Be(2);
        p.PhotoId.Should().Be(1);
        p.UpdatedOn.Should().BeNull(); 
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetName_ShouldThrowError_WhenInputIsMissing(string? name)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 1m, 1, 1, 1);
        
        // Act
        Action act = () => p.SetName(name!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*Name is required.*");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(67)]
    public void SetName_Throws_WhenTooLong(int length)
    {
        // Arrange
        var entity = new ProductEntity("x", "d", 1m, 1, 1, 1);
        var name = new string('a', length);
        
        // Act
        Action act = () => entity.SetName(name);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(name)} too long*");
    }

    [Theory]
    [InlineData("  hello    world", "hello world", "HELLO WORLD")]
    [InlineData("cool Shirt   ", "cool Shirt", "COOL SHIRT")]
    [InlineData("    TEST    this ", "TEST this", "TEST THIS")]
    public void SetName_ShouldNormaliseField_WhenInputIsValid(string name, string expected, string normalised)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 1m, 1, 1, 1);

        // Act
        p.SetName(name);

        // Assert
        p.Name.Should().Be(expected);
        p.NameNormalized.Should().Be(normalised);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void ChangeUnitPrice_ShouldThrowError_WhenInputIsNegative(decimal price)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 1m, 1, 1, 1);
        
        // Act
        Action act = () => p.ChangeUnitPrice(price);
        
        // Arrange
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(price)} cannot be negative*");
    }

    [Fact]
    public void ChangeUnitPrice_ShouldSetPrice_WhenInputIsValid()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);

        // Act
        p.ChangeUnitPrice(12.34m);

        // Arrange
        p.UnitPrice.Should().Be(12.34m);
    }

    [Fact]
    public void ChangeUnitPrice_ShouldDoNothing_WhenInputIsTheSameValue()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);
        var before = p.UpdatedOn;

        // Act
        p.ChangeUnitPrice(10m);

        // Assert
        p.UnitPrice.Should().Be(10m);
        p.UpdatedOn.Should().Be(before);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseStock_ShouldThrowError_WhenInputIsNonPositive(int quantity)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);

        // Act
        Action act = () => p.IncreaseStock(quantity);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive*");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(99)]
    public void SetStock_ShouldIncreaseStock_WhenInputIsValid(int stock)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 5, 1, 1);

        // Act
        p.SetStock(stock);

        // Assert
        p.QuantityInStock.Should().Be(stock);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(99)]
    public void SetStock_DecreaseStock_WhenInputIsValid(int stock)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 100, 1, 1);

        // Act
        p.SetStock(stock);

        // Assert
        p.QuantityInStock.Should().Be(stock);
    }

    [Fact]
    public void IncreaseStock_ShouldAdd_WhenInputIsValid()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 2, 1, 1);

        // Act
        p.IncreaseStock(3);

        // Assert
        p.QuantityInStock.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseStock_Throws_WhenNonPositive(int quantity)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 2, 1, 1);
        
        // Act
        Action act = () => p.DecreaseStock(quantity);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(quantity)} must be positive.*");
    }

    [Fact]
    public void DecreaseStock_ShouldThrowError_WhenInsufficient()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 2, 1, 1);
        
        // Act
        Action act = () => p.DecreaseStock(3);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Insufficient stock.*");
    }

    [Fact]
    public void DecreaseStock_ShouldSubtract_WhenSufficientStock()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 5, 1, 1);

        // Act
        p.DecreaseStock(2);

        // Assert
        p.QuantityInStock.Should().Be(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetPhoto_ShouldThrowError_WhenInputIsNonPositive(int photoId)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);
        var photo = new PhotoEntity("pic.jpg", "p1", "https://test.example.com/pic.jpg");

        // Act
        Action act = () => p.SetPhoto(photoId, photo);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(photoId)} is invalid*");
    }

    [Fact]
    public void SetPhoto_ShouldSetPhoto_WhenInputIsNotNull()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);
        var photo = new PhotoEntity("pic.jpg", "p1", "https://test.example.com/pic.jpg");

        // Act
        p.SetPhoto(42, photo);

        // Assert
        p.PhotoId.Should().Be(42);
        p.Photo.Should().BeSameAs(photo);
    }

    [Fact]
    public void SetPhoto_ShouldSetId_WhenPhotoReferenceNull()
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);
        
        // Act
        p.SetPhoto(7);

        // Assert
        p.PhotoId.Should().Be(7);
        p.Photo.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetDescription_ShouldThrowError_WhenInputIsMissing(string? description)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);
        
        // Act
        Action act = () => p.SetDescription(description!);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(description)} is required.*");
    }

    [Theory]
    [InlineData("   Test   ", "Test")]
    [InlineData("Testing      ", "Testing")]
    [InlineData("  Hello  There  ", "Hello  There")]
    public void SetDescription_ShouldTrim_WhenInputIsValid(string description, string expected)
    {
        // Arrange
        var p = new ProductEntity("x", "d", 10m, 1, 1, 1);

        // Act
        p.SetDescription(description);

        // Assert
        p.Description.Should().Be(expected);
    }
}
