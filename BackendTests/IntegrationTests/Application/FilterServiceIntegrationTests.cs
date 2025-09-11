namespace BackendTests.IntegrationTests.Application;

public class FilterServiceIntegrationTests : SqlDbTestBase
{
    [Fact]
    public async Task GetProductFilters_ShouldReturnEmptyLists_WhenNoProducts()
    {
        // Arrange
        using var context = CreateContext();
        var service = new FilterService(context, Mapper);

        // Act
        var result = await service.GetProductFilters();

        // Assert
        result.Brands.Should().BeEmpty();
        result.ProductTypes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductFilters_ShouldReturnDistinctBrandsAndTypes()
    {
        // Arrange
        using var context = CreateContext();
        var service = new FilterService(context, Mapper);
        
        var brand1 = new BrandEntity("Nike");
        var brand2 = new BrandEntity("Adidas");
        var type1 = new ProductTypeEntity("Shoes");
        var type2 = new ProductTypeEntity("Clothing");
        var photo1 = new PhotoEntity("photo1", "123456789", "https://test/com/photo1.jpg");
        var photo2 = new PhotoEntity("photo2", "1200089", "https://test/com/photo2.jpg");
        var photo3 = new PhotoEntity("photo3", "123489000", "https://test/com/photo3.jpg");

        await SeedAsync(brand1, brand2);
        await SeedAsync(type1, type2);
        await SeedAsync(photo1, photo2, photo3);

        var product1 = new ProductEntity("sku1", "Prod1", 100m, 10, type1.Id, brand1.Id, photo1.Id);
        var product2 = new ProductEntity("sku2", "Prod2", 200m, 10, type2.Id, brand1.Id, photo2.Id);
        var product3 = new ProductEntity("sku3", "Prod3", 300m, 10, type1.Id, brand2.Id, photo3.Id);

        await SeedAsync(product1, product2, product3);

        // Act
        var result = await service.GetProductFilters();

        // Assert
        result.Brands.Should().HaveCount(2);
        result.Brands.Select(b => b.Name).Should().BeEquivalentTo(new[] { "Nike", "Adidas" });
        result.ProductTypes.Should().HaveCount(2);
        result.ProductTypes.Select(t => t.Name).Should().BeEquivalentTo(new[] { "Shoes", "Clothing" });
    }

    [Fact]
    public async Task GetProductFilters_ShouldIgnoreDuplicates()
    {
        // Arrange
        using var context = CreateContext();
        var service = new FilterService(context, Mapper);

        var brand = new BrandEntity("Puma");
        var type = new ProductTypeEntity("Shoes");
        var photo = new PhotoEntity("photo1", "123456789", "https://test/com/photo1.jpg");

        await SeedAsync(brand);
        await SeedAsync(type);
        await SeedAsync(photo);

        var product1 = new ProductEntity("sku1", "P1", 50m, 5, brand.Id, type.Id, photoId: photo.Id, brand: brand, type: type);
        var product2 = new ProductEntity("sku2", "P2", 75m, 5, brand.Id, type.Id, photoId: photo.Id, brand: brand, type: type);

        await SeedAsync(product1, product2);

        // Action
        var result = await service.GetProductFilters();

        // Assert
        result.Brands.Should().ContainSingle(b => b.Name == "Puma");
        result.ProductTypes.Should().ContainSingle(t => t.Name == "Shoes");
    }
}
