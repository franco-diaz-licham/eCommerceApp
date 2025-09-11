namespace BackendTests.UnitTests.Domain.Entities;

public class OrderStatusEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetName_WhenInstantiated()
    {
        // Arrange and Act
        var status = new OrderStatusEntity("  in   progress  ");

        // Assert
        status.Name.Should().Be("in progress");
        status.NameNormalized.Should().Be("IN PROGRESS");
        status.IsActive.Should().BeTrue();
        status.UpdatedOn.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowError_WhenInputIsMissing(string? name)
    {
        // Arrange and Act
        Action act = () => new OrderStatusEntity(name!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(name)} is required*");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    public void SetName_ShouldThrowError_WhenInputTooLong(int length)
    {
        // Arrange
        var status = new OrderStatusEntity("ok");
        var name = new string('a', length);

        // Act
        Action act = () => status.SetName(name);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(name)} too long*");
    }

    [Theory]
    [InlineData("peNding", "peNding", "PENDING")]
    [InlineData("   cancelled", "cancelled",  "CANCELLED")]
    [InlineData("    shipped   out   ", "shipped out",  "SHIPPED OUT")]
    [InlineData("PAid   ", "PAid", "PAID")]
    public void SetName_ShouldUpdateNameAndNormalise_WhenInputsAreValid(string name, string expected, string normalised)
    {
        // Arrange
        var status = new OrderStatusEntity("pending");

        // Act
        status.SetName(name);

        // Assert
        status.Name.Should().Be(expected);
        status.NameNormalized.Should().Be(normalised);
    }

    [Fact]
    public void Activate_ShouldDoNothing_WhenAlreadyActive()
    {
        // Arrange
        var status = new OrderStatusEntity("pending");

        // Act
        status.Activate();

        // Assert
        status.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldSetInactive_WhenCalled()
    {
        // Arrange
        var status = new OrderStatusEntity("pending");

        // Act
        status.Deactivate();

        // Assert
        status.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldTogglesBackToActive_WhenCalled()
    {
        // Arrange
        var status = new OrderStatusEntity("pending");
        status.Deactivate();

        // Act
        status.Activate();

        // Assert
        status.IsActive.Should().BeTrue();
    }
}
