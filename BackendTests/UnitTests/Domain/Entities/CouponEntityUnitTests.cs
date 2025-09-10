namespace BackendTests.UnitTests.Domain.Entities;

public class CouponEntityUnitTests
{
    [Fact]
    public void CreateAmountOff_ShouldSetCoreFields_WhenInputsAreValid()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = CouponEntity.CreateAmountOff("  summer  sale  ", "remote-1", "  SUM10  ", 10m);

        // Assert
        entity.Name.Should().Be("summer sale");
        entity.NameNormalized.Should().Be("SUMMER SALE");
        entity.PromotionCode.Should().Be("SUM10");
        entity.PromotionCodeNormalized.Should().Be("SUM10");
        entity.RemoteId.Should().Be("remote-1");
        entity.AmountOff.Should().Be(10m);
        entity.PercentOff.Should().BeNull();
        entity.IsActive.Should().BeTrue();
        entity.CreatedOn.Should().BeOnOrAfter(now);
        entity.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void CreatePercentOff_ShouldSetFields_WhenInputesAreValid()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = CouponEntity.CreatePercentOff("  ten  percent  ", "r2", "  P10  ", 10m);

        // Arrange
        entity.Name.Should().Be("ten percent");
        entity.NameNormalized.Should().Be("TEN PERCENT");
        entity.PromotionCode.Should().Be("P10");
        entity.PromotionCodeNormalized.Should().Be("P10");
        entity.RemoteId.Should().Be("r2");
        entity.PercentOff.Should().Be(10m);
        entity.AmountOff.Should().BeNull();
        entity.IsActive.Should().BeTrue();
        entity.CreatedOn.Should().BeOnOrAfter(now);
        entity.UpdatedOn.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetName_ShouldThrowError_WhenInputIsMissing(string? name)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);

        // Act
        Action act = () => entity.SetName(name!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(name)} is required.*");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(99)]
    public void SetName_ShouldThrowError_WhenNameTooLong(int length)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        var name = new string('a', length);

        // Act
        Action act = () => entity.SetName(name);

        // Arrange
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(name)} too long.*");
    }

    [Theory]
    [InlineData("  hello    world  ", "hello world", "HELLO WORLD")]
    [InlineData(" test TSd    ", "test TSd", "TEST TSD")]
    [InlineData("Christmas 2026   Code   ", "Christmas 2026 Code", "CHRISTMAS 2026 CODE" )]
    public void SetName_ShouldNormalizeInput_WhenInputIsValid(string name, string expectedName, string normName)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        var before = entity.UpdatedOn;

        // Act
        entity.SetName(name);

        // Assert
        entity.Name.Should().Be(expectedName);
        entity.NameNormalized.Should().Be(normName);
        entity.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetPromotionCode_ShouldThrowError_WhenInputIsMissing(string? code)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);

        // Act
        Action act = () => entity.SetPromotionCode(code!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(code)} is required.*");
    }

    [Theory]
    [InlineData(65)]
    [InlineData(66)]
    [InlineData(99)]
    public void SetPromotionCode_ShouldThrowErro_WhenCodeTooLong(int length)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        var code = new string('a', length);

        // Act
        Action act = () => entity.SetPromotionCode(code);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(code)} too long.*");
    }

    [Theory]
    [InlineData(" Summer2026 ", "Summer2026", "SUMMER2026")]
    [InlineData("     winter-x  ", "winter-x", "WINTER-X")]
    [InlineData("X-mas-2026      ", "X-mas-2026", "X-MAS-2026")]
    public void SetPromotionCode_ShouldNormalizeInput_WhenInputIsValid(string code, string expectedCode, string normCode)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        var before = entity.UpdatedOn;

        // Act
        entity.SetPromotionCode(code);

        // Assert
        entity.PromotionCode.Should().Be(expectedCode);
        entity.PromotionCodeNormalized.Should().Be(normCode);
        entity.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetRemoteId_ShouldThrowError_WhenInputIsMissing(string? id)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);

        // Act
        Action act = () => entity.SetRemoteId(id!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage($"*{nameof(id)} is required.*");
    }

    [Theory]
    [InlineData(" stripe_123     ", "stripe_123")]
    [InlineData("     test  1234 ", "test  1234")]
    [InlineData("code12  23 2      ", "code12  23 2")]
    public void SetRemoteId_ShouldTrimInput_WhenInputIsValid(string id, string expectedId)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        var before = entity.UpdatedOn;

        // Act
        entity.SetRemoteId(id);

        // Assert
        entity.RemoteId.Should().Be(expectedId);
        entity.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void SetAmountOff_ShouldThrowError_WhenInputIsNegative(decimal amount)
    {
        // Arrange
        var entity = CouponEntity.CreatePercentOff("ok", "r", "code", 5m);
        
        // Act
        Action act = () => entity.SetAmountOff(amount);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(amount)} cannot be negative.*");
    }

    [Theory]
    [InlineData(100.0)]
    [InlineData(99.999)]
    public void SetAmountOff_ShouldSetAndClearsPercent_WhenInputIsValid(decimal amount)
    {
        // Arrange
        var entity = CouponEntity.CreatePercentOff("ok", "r", "code", 5m);
        var before = entity.UpdatedOn;

        // Act
        entity.SetAmountOff(amount);

        // Assert
        entity.AmountOff.Should().Be(amount);
        entity.PercentOff.Should().BeNull();
        entity.UpdatedOn.Should().NotBe(before);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    [InlineData(200.1)]
    public void SetPercentOff_ShouldThrowError_WhenInputIsNegative(decimal percent)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 1m);
        
        // Act
        Action act = () => entity.SetPercentOff(percent);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(percent)} must be between 0 and 100.*");
    }

    [Theory]
    [InlineData(100.0)]
    [InlineData(15.21)]
    [InlineData(65.5)]
    [InlineData(99.9)]
    public void SetPercentOff_ShouldSetClearAmount_WhenInputIsValid(decimal percent)
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("ok", "r", "code", 10m);
        var before = entity.UpdatedOn;

        // Act
        entity.SetPercentOff(percent);

        // Assert
        entity.PercentOff.Should().Be(percent);
        entity.AmountOff.Should().BeNull();
        entity.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void Activate_ShouldDoNothing_WhenActivated()
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("x", "r", "code", 1m);
        var before = entity.UpdatedOn;

        // Act
        entity.Activate();

        // Arrange
        entity.IsActive.Should().BeTrue();
        entity.UpdatedOn.Should().Be(before);
    }

    [Fact]
    public void Activate_ShouldActivate_WhenDeactivated()
    {
        // Arrange
        var c = CouponEntity.CreateAmountOff("x", "r", "code", 1m);
        c.Deactivate();
        var before = c.UpdatedOn;

        // Act
        c.Activate();

        // Assert
        c.IsActive.Should().BeTrue();
        c.UpdatedOn.Should().NotBe(before);
    }

    [Fact]
    public void Deactivate_ShouldSetInactive_WhenActivated()
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("x", "r", "code", 1m);
        var before = entity.UpdatedOn;

        // Act
        entity.Deactivate();

        // Assert
        entity.IsActive.Should().BeFalse();
        entity.UpdatedOn.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldDoNothing_WhenDiactivated()
    {
        // Arrange
        var entity = CouponEntity.CreateAmountOff("x", "r", "code", 1m);
        entity.Deactivate();

        // Act
        entity.Deactivate();

        // Assert
        entity.IsActive.Should().BeFalse();
        entity.UpdatedOn.Should().NotBeNull();
    }

    [Theory]
    [InlineData(200.0, 50.0, 50.0)]
    [InlineData(400.0, 60.0, 60.0)]
    [InlineData(40.0, 40.0, 40.0)]
    [InlineData(30.0, 40.0, 30.0)]
    [InlineData(15.0, 65.0, 15.0)]
    public void CalculateDiscount_ShouldCalculateAmountOff_WhenCalled(decimal subtotal, decimal amountOff, decimal expected)
    {
        // Arrange and Act
        var entity = CouponEntity.CreateAmountOff("x", "r", "code", amountOff);

        // Assert
        entity.CalculateDiscount(subtotal).Should().Be(expected);
    }

    [Theory]
    [InlineData(79.99, 12.5, 10.00)]     // 12.5% of 79.99  = 9.99875 => 10.00  (2 d.p.)
    [InlineData(5.0, 12.5, 0.63)]        // 12.5% of 5      = 0.625   => 0.63   (2 d.p.)
    [InlineData(0.0, 12.5, 0.00)]        // 12.5% of 0      = 0.000   => 0.00   (2 d.p.)
    [InlineData(100.0, 100.0, 100.00)]   // 100.0% of 100.0 = 100.00  => 100.00 (2 d.p.)
    public void CalculateDiscount_ShouldCalculatePercentOff_WhenCalled(decimal subtotal, decimal percentOff, decimal expected)
    {
        // Arrange and Act
        var entity = CouponEntity.CreatePercentOff("x", "r", "code", percentOff);
        
        // Assert
        entity.CalculateDiscount(subtotal).Should().Be(expected);
        entity.CalculateDiscount(subtotal).Should().Be(expected); 
        entity.CalculateDiscount(subtotal).Should().Be(expected);
    }

    [Fact]
    public void CalculateDiscount_ShouldThrowError_WhenInactive()
    {
        // Arrange
        var c = CouponEntity.CreateAmountOff("x", "r", "code", 10m);
        c.Deactivate();

        // Act
        Action act = () => c.CalculateDiscount(100m);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*Coupon inactive.*");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-5)]
    public void CalculateDiscount_SholdThrowError_WhenSubtotalNegative(decimal subtotal)
    {
        // Arrange
        var c = CouponEntity.CreateAmountOff("x", "r", "code", 10m);

        // Act
        Action act = () => c.CalculateDiscount(subtotal);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{nameof(subtotal)} cannot be negative.*");
    }
}
