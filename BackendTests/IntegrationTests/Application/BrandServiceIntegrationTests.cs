namespace BackendTests.IntegrationTests.Application;

public class BrandServiceIntegrationTests : SqlDbTestBase
{
    [Fact]
    public async Task GetAsync_ShouldReturnsDto_WhenFound()
    {
        // Arrange
        using var context = CreateContext();
        var service = new BrandService(context, Mapper);

        var brand = new BrandEntity("levi");
        await SeedAsync(brand);

        // Act
        var result = await service.GetAsync(brand.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(brand.Id);
        result.Value!.Name.Should().Be("levi");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        using var context = CreateContext();
        var service = new BrandService(context, Mapper);

        // Act
        var result = await service.GetAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error?.Message.Should().Contain("Brand not found");
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplySearchAndSortAndPaging_WhenCalled()
    {
        // Arrange
        using var context = CreateContext();
        var service = new BrandService(context, Mapper);

        var brands = new[]
        {
            new BrandEntity("apple"),
            new BrandEntity("levi"),
            new BrandEntity("zeta"),
            new BrandEntity("alpha")
        };
        await SeedAsync(brands);

        var specs = new BaseQuerySpecs
        {
            SearchTerm = "a",                
            OrderBy = "nameAsc",           
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var page = await service.GetAllAsync(specs);

        // Assert
        page.IsSuccess.Should().Be(true);
        page.Type.Should().Be(ResultTypeEnum.Success);
        page.Error.Should().BeNull();
        page.Value.Should().HaveCount(3);
        var names = page.Value.Select(x => x.Name).ToArray();
        names.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_ShouldPageContent_WhenCalled()
    {
        // Arrange
        using var context = CreateContext();
        var service = new BrandService(context, Mapper);

        await SeedAsync(
            new BrandEntity("a1"), 
            new BrandEntity("a2"),
            new BrandEntity("a3"), 
            new BrandEntity("a4"),
            new BrandEntity("a5")
        );

        // Act
        var specs = new BaseQuerySpecs { PageNumber = 2, PageSize = 2, OrderBy = "nameAsc" };
        var page = await service.GetAllAsync(specs);

        // Assert
        page.Value.Should().HaveCount(2);
        page.IsSuccess.Should().Be(true);
        page.Type.Should().Be(ResultTypeEnum.Success);
    }
}
