namespace BackendTests.UnitTests.Domain.Entities;

public class AddressEntityUnitTests
{
    [Fact]
    public void Constructor_ShouldSetFields_WhenGivenValidInput()
    {
        // Arrange and Act
        var entity = new AddressEntity(" 123   Main St ", " Apt  4 ", " Sydney ", " NSW ", "2000", "au");

        // Assert
        entity.Line1.Should().Be("123 Main St");
        entity.Line2.Should().Be("Apt 4");
        entity.City.Should().Be("Sydney");
        entity.State.Should().Be("NSW");
        entity.PostalCode.Should().Be("2000");
        entity.Country.Should().Be("AU");
    }

    [Fact]
    public void Update_ShouldReplaceFields_WhenGivenValidInput()
    {
        // Arrange
        var entity = new AddressEntity("1 St", null, "City", "NSW", "1234", "AU");

        // Act
        entity.Update("2 St", "Unit 9", "Melb", "VIC", "3000", "nz");

        // Assert
        entity.Line1.Should().Be("2 St");
        entity.Line2.Should().Be("Unit 9");
        entity.City.Should().Be("Melb");
        entity.State.Should().Be("VIC");
        entity.PostalCode.Should().Be("3000");
        entity.Country.Should().Be("NZ");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetLine1_ShouldThrowError_WhenInputIsInvalid(string? val)
    {
        // Asserts
        var entity = new AddressEntity("x", null, "City", "NSW", "1234", "AU");

        // Act
        Action act = () => entity.SetLine1(val!);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.Line1)} is required*");
    }

    [Theory]
    [InlineData(127)]
    [InlineData(128)]
    public void SetLine1_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("x", null, "City", "NSW", "1234", "AU");

        // Act
        entity.SetLine1(val);

        // Assert
        entity.Line1.Should().Be(val);
    }

    [Theory]
    [InlineData(129)]
    [InlineData(130)]
    public void SetLine1_ShouldThrowError_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("x", null, "City", "NSW", "1234", "AU");

        // Act and Assert
        Action act = () => entity.SetLine1(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.Line1)} too long.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetLine2_ShouldAllowNullOrWhitespace_WhenInputIsValid(string? val)
    {
        // Arrange
        var entity = new AddressEntity("x", "abc", "City", "NSW", "1234", "AU");

        // Act
        entity.SetLine2(val);

        // Assert
        entity.Line2.Should().BeNull();
    }

    [Theory]
    [InlineData(127)]
    [InlineData(128)]
    public void SetLine2_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("Line1", null, "City", "NSW", "1234", "AU");

        // Act and Assert
        entity.SetLine2(val);

        // Assert
        entity.Line2.Should().Be(val);
    }

    [Theory]
    [InlineData(129)]
    [InlineData(130)]
    public void SetLine2_ShouldReturnExpectedResult_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("x", null, "City", "NSW", "1234", "AU");

        // Act
        Action act = () => entity.SetLine2(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.Line2)} too long.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetCity_ShouldThrowError_WhenInputIsInvalid(string? val)
    {
        // Arrange
        var entity = new AddressEntity("x", "y", "City", "NSW", "1234", "AU");

        // Act
        Action act = () => entity.SetCity(val!);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.City)} is required*");
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    public void SetCity_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("x", null, "City", "ST", "1234", "AU");

        // Act
        entity.SetState(val);

        // Assert
        entity.State.Should().Be(val);
    }

    [Theory]
    [InlineData(13)]
    [InlineData(14)]
    public void SetCity_ShouldThrowError_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var val = new string('x', len);
        var entity = new AddressEntity("Line1", null, "x", "ST", "1234", "AU");

        // Act
        Action act = () => entity.SetCity(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.City)} too long.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetState_ShouldThrowError_WhenInputIsInvalid(string? val)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "x", "1234", "AU");

        // Act
        Action act = () => entity.SetState(val!);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.State)} is required*");
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    public void SetState_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "x", "1234", "au");
        string val = new('a', len);

        // Act
        entity.SetState(val);

        // Assert
        entity.State.Should().Be(val);
    }

    [Theory]
    [InlineData(14)]
    [InlineData(13)]
    public void SetState_ShouldThrowError_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var entity = new AddressEntity("x", null, "City", "x", "1234", "au");
        string val = new('a', len);

        // Act
        Action act = () => entity.SetState(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.State)} too long.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetPostalCode_ShouldThrowError_WhenInputIsInvalid(string? val)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "x", "1234", "AU");

        // Act
        Action act = () => entity.SetPostalCode(val!);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.PostalCode)} is required*");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void SetPostalCode_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "x", "1234", "au");
        string val = new('a', len);

        // Act
        entity.SetPostalCode(val);

        // Assert
        entity.PostalCode.Should().Be(val);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    public void SetPostalCode_ShouldThrowError_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "State", "x", "au");
        string val = new('a', len);

        // Act
        Action act = () => entity.SetPostalCode(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.PostalCode)} too long.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetCountry_ShouldThrowError_WhenInputIsInvalid(string? val)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "State", "1234", "x");

        // Act
        Action act = () => entity.SetCountry(val!);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.Country)} is required*");
    }

    [Theory]
    [InlineData(55)]
    [InlineData(56)]
    public void SetCountry_ShouldSetField_WhenInputLengthIsValid(int len)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "State", "1234", "x");
        string val = new('A', len);

        // Act
        entity.SetCountry(val);

        // Assert
        entity.Country.Should().Be(val);
    }

    [Theory]
    [InlineData(57)]
    [InlineData(58)]
    public void SetCountry_ShouldThrowError_WhenInputLengthIsTooLong(int len)
    {
        // Arrange
        var entity = new AddressEntity("Line1", null, "City", "State", "1234", "x");
        string val = new('A', len);

        // Act
        Action act = () => entity.SetCountry(val);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(AddressEntity.Country)} too long.*");
    }

    [Theory]
    [InlineData("aud", "AUD")]
    [InlineData("usa", "USA")]
    [InlineData("nz", "NZ")]
    public void SetCountry_ShouldUppercaseInput_WhenInputIsLowercase(string val, string expected)
    {
        // Arrange
        var entity = new AddressEntity("L1", null, "City", "ST", "1234", "x");
        
        // Act
        entity.SetCountry(val);
        
        // Assert
        entity.Country.Should().Be(expected);
    }

    [Fact]
    public void Setters_ShouldCollapseInternalSpaces_WhenInputIsValid()
    {
        var entity = new AddressEntity(" L  X ", " Y  Z ", " Syd   ney ", " N   S   W ", "2000", "AU");
        entity.SetCity("  Mel    bourne ");
        entity.SetState(" V   I   C ");
        entity.Line1.Should().Be("L X");
        entity.Line2.Should().Be("Y Z");
        entity.City.Should().Be("Mel bourne");
        entity.State.Should().Be("V I C");
    }
}