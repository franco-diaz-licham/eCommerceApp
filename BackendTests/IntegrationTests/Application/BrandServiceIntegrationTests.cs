namespace BackendTests.IntegrationTests.Application;

public class BrandServiceIntegrationTests : SqlDbTestBase
{
    private async Task SeedAsync(params BrandEntity[] brands)
    {
        using var ctx = Context();
        ctx.Brands.AddRange(brands);
        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnsDto_WhenFound()
    {
        // Arrange
        var b1 = new BrandEntity("acme");
        await SeedAsync(b1);

        using var ctx = Context();
        var svc = new BrandService(ctx, Mapper);

        // Act
        var result = await svc.GetAsync(b1.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(b1.Id);
        result.Value!.Name.Should().Be("acme");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        using var ctx = Context();
        var svc = new BrandService(ctx, Mapper);

        // Act
        var result = await svc.GetAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Type.Should().Be(ResultTypeEnum.NotFound);
        result.Error?.Message.Should().Contain("Brand not found");
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplySearchAndSortAndPaging_WhenCalled()
    {
        // Arrange
        var brands = new[]
        {
            new BrandEntity("apple"),
            new BrandEntity("acme"),
            new BrandEntity("zeta"),
            new BrandEntity("alpha")
        };
        await SeedAsync(brands);

        using var ctx = Context();
        var svc = new BrandService(ctx, Mapper);
        var specs = new BaseQuerySpecs
        {
            SearchTerm = "a",                
            OrderBy = "nameAsc",           
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var page = await svc.GetAllAsync(specs);

        // Assert
        page.IsSuccess.Should().Be(true);
        page.Type.Should().Be(ResultTypeEnum.Success);
        page.Error.Should().BeNull();
        page.Value.Should().HaveCount(4);
        var names = page.Value.Select(x => x.Name).ToArray();
        names.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_ShouldPageContent_WhenCalled()
    {
        // Arrange
        await SeedAsync(
            new BrandEntity("a1"), 
            new BrandEntity("a2"),
            new BrandEntity("a3"), 
            new BrandEntity("a4"),
            new BrandEntity("a5")
        );

        // Act
        using var ctx = Context();
        var svc = new BrandService(ctx, Mapper);
        var specs = new BaseQuerySpecs { PageNumber = 2, PageSize = 2, OrderBy = "nameAsc" };
        var page = await svc.GetAllAsync(specs);

        // Assert
        page.Value.Should().HaveCount(2);
        page.IsSuccess.Should().Be(true);
        page.Type.Should().Be(ResultTypeEnum.Success);
    }
}
