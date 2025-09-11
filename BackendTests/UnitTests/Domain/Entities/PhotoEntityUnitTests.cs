namespace BackendTests.UnitTests.Domain.Entities;

public class PhotoEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetFields_WhenInstantiated()
    {
        // Arrange and Act
        var photo = new PhotoEntity("  my photo  .jpg  ", "  pub123  ", "https://testurl.com.au");

        // Assert
        photo.FileName.Should().Be("my photo .jpg");
        photo.PublicId.Should().Be("pub123");
        photo.PublicUrl.Should().Be("https://testurl.com.au");
        photo.UpdatedOn.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetFileName_ShouldThrowError_WhenMissing(string? fileName)
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://testurl.com.au");

        // Act
        Action act = () => photo.SetFileName(fileName!);

        // Arrange
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(fileName)} is required.*");
    }

    [Theory]
    [InlineData(129)]
    [InlineData(130)]
    public void SetFileName_ShouldThrowError_WhenInputTooLong(int length)
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://testurl.com.au");
        var fileName = new string('x', length);

        // Act
        Action act = () => photo.SetFileName(fileName);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(fileName)} too long*");
    }

    [Theory]
    [InlineData("  hello    world.png  ", "hello world.png")]
    [InlineData("image.jpg   ", "image.jpg")]
    [InlineData("      photo.png", "photo.png")]
    [InlineData("    test     photo.png    ", "test photo.png")]
    public void SetFileName_SouldSetFields_WhenInputsAreValid(string fileName, string expected)
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://testurl.com.au");

        // Act
        photo.SetFileName(fileName);

        // Assert
        photo.FileName.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, "https://test.com/a.jpg", "Public Id is required.")]
    [InlineData("   ", "https://test.com/a.jpg", "Public Id is required.")]
    [InlineData("id", "not-a-url", "PublicUrl must be an absolute URL.")]
    [InlineData("id", "http://test.com/a.jpg", "PublicUrl must use HTTPS.")]
    public void ReplaceRemote_ShouldThrowError_WhenInvalidInputs(string? id, string url, string message)
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://test.com/a.jpg");

        // Act
        Action act = () => photo.ReplaceRemote(id!, url);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{message}*");
    }

    [Fact]
    public void ReplaceRemote_ShouldSetFields_WhenInputsAreValid()
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://test.com/a.jpg");

        // Act
        photo.ReplaceRemote("newId", "https://test.com/b.jpg");

        // Assert
        photo.PublicId.Should().Be("newId");
        photo.PublicUrl.Should().Be("https://test.com/b.jpg");
    }

    [Fact]
    public void CollapseSpaces_ShouldRemovesExtraSpaces_WhenCalled()
    {
        // Arrange
        var photo = new PhotoEntity("a.jpg", "id", "https://test.com/a.jpg");

        // Act
        photo.SetFileName("   my     spaced   file.jpg  ");

        // Assert
        photo.FileName.Should().Be("my spaced file.jpg");
    }
}
