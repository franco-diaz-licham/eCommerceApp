namespace BackendTests.UnitTests.Application;

public class SearchProviderUnitTests
{
    [Fact]
    public void SearchProvider_ShouldFilterByName_WhenCalled()
    {
        // Arrange
        var data = new[]
        {
            new BrandEntity("Apple"),
            new BrandEntity("Beta"),
            new BrandEntity("Gamma")
        }.AsQueryable();
        var specs = new BaseQuerySpecs { SearchTerm = "a" };
        var context = new QueryStrategyContext<BrandEntity>(
            new SearchEvaluatorStrategy<BrandEntity>(specs.SearchTerm, new BrandSearchProvider()),
            new SortEvaluatorStrategy<BrandEntity>("name_asc", new BrandSortProvider())
        );

        // Act
        var filtered = context.ApplyQuery(data).ToList();

        // Assert
        filtered.Should().Contain(b => b.Name == "Apple").And.Contain(b => b.Name == "Gamma");
    }
}
