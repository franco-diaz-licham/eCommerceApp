namespace BackendTests.UnitTests.Domain.Entities;

public class ProductTypeEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetsCoreFields_WhenInstantiated()
    {
        // Arrange and Act
        var type = new ProductTypeEntity("  classic   Shirts  ");

        // Assert
        type.Name.Should().Be("classic Shirts");
        type.NameNormalized.Should().Be("CLASSIC SHIRTS");
        type.IsActive.Should().BeTrue();
        type.UpdatedOn.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowError_WhenInputIsMissing(string? name)
    {
        // Arrange and Act
        Action act = () => new ProductTypeEntity(name!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(name)} is required*");
    }

    [Theory]
    [InlineData("Jeans  ", "Jeans", "JEANS")]
    [InlineData("   ShirtS", "ShirtS", "SHIRTS")]
    [InlineData("    PoLo   ShiRT   ", "PoLo ShiRT", "POLO SHIRT")]
    public void SetName_ShouldUpdateName_WhenInputIsValid(string name, string expected, string normalised)
    {
        // Arrange
        var entity = new ProductTypeEntity("x");

        // Act
        entity.SetName(name);

        // Assert
        entity.Name.Should().Be(expected);
        entity.NameNormalized.Should().Be(normalised);
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(99)]
    public void SetName_ShouldThrowError_WhenInputIsTooLong(int length)
    {
        // Arrange
        var entity = new ProductTypeEntity("x");
        var name = new string('a', length);

        // Act
        Action act = () => entity.SetName(name);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(name)} is too long*");
    }

    [Fact]
    public void Activate_ShouldDoNothing_WhenAlreadyActive()
    {
        // Arrange
        var type = new ProductTypeEntity("x");

        // Act
        type.Activate();

        // Assert
        type.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldSetInactive_WhenActivated()
    {
        // Arrange
        var type = new ProductTypeEntity("x");

        // Act
        type.Deactivate();

        // Assert
        type.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldTogglesBackToActive_WhenDeactivated()
    {
        // Arrange
        var type = new ProductTypeEntity("x");

        // Act
        type.Deactivate();
        type.Activate();

        // Assert
        type.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CollapseSpaces_ShouldNormalise_WhenCalled()
    {
        // Arrange
        var type = new ProductTypeEntity("x");

        // Act
        type.SetName("  cowboy    jeans   ");

        // Assert
        type.Name.Should().Be("cowboy jeans");
        type.NameNormalized.Should().Be("COWBOY JEANS");
    }
}
