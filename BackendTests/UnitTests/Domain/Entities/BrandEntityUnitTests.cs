namespace BackendTests.UnitTests.Domain.Entities;

public class BrandEntityUnitTests
{
    [Theory]
    [InlineData("Levi's" , "Levi's", "LEVI'S")]
    [InlineData("acme    inc", "acme inc", "ACME INC")]
    [InlineData(" ToMmY hiLlfigger  ", "ToMmY hiLlfigger", "TOMMY HILLFIGGER")]
    public void Constructor_ShouldSetName_WhenInstantiated(string name, string expectedName, string expectedNormalisedName)
    {
        // Arrange and Act
        var brand = new BrandEntity(name);

        // Assert
        brand.Name.Should().Be(expectedName);
        brand.NameNormalized.Should().Be(expectedNormalisedName);
        brand.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ThrowsError_WhenNameIsMissing(string? name)
    {
        // Act
        Action act = () => new BrandEntity(name!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(name)} is required.*");
    }

    [Fact]
    public void SetName_ShouldUpdateName_WhenInputIsValid()
    {
        // Arrange
        var brand = new BrandEntity("acme");

        // Act
        brand.SetName("  acme   corp  ");

        // Assert
        brand.Name.Should().Be("acme corp");
        brand.NameNormalized.Should().Be("ACME CORP");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(67)]
    public void SetName_ShouldThrowError_WhenNameIsTooLong(int length)
    {
        // Arrange
        var brand = new BrandEntity("brand");
        var name = new string('a', length);

        // Act
        Action act = () => brand.SetName(name);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(name)} too long.*");
    }

    [Fact]
    public void Activate_ShouldDoNothing_WhenAlreadyActive()
    {
        // Arrange
        var brand = new BrandEntity("acme");

        // Act
        brand.Activate();

        // Assert
        brand.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldInactivate_WhenActivated()
    {
        // Arrange
        var brand = new BrandEntity("acme");

        // Act
        brand.Deactivate();

        // Assert
        brand.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldActivate_WhenDeActivated()
    {
        // Arrange
        var brand = new BrandEntity("acme");
        brand.Deactivate();

        // Act
        brand.Activate();

        // Assert
        brand.IsActive.Should().BeTrue();
    }
}
