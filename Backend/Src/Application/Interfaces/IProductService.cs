namespace Backend.Src.Application.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Method creates a product.
    /// </summary>
    Task<ProductDto> CreateAsync(ProductCreateDto Dto);

    /// <summary>
    /// Method which deletes a product.
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Method which gets all products.
    /// </summary>
    Task<Result<List<ProductDto>>> GetAllAsync(ProductQuerySpecs specs);

    /// <summary>
    /// Method which gets a Product.
    /// </summary>
    Task<ProductDto?> GetAsync(int id);

    /// <summary>
    /// Method which updates a product.
    /// </summary>
    Task<ProductDto> UpdateAsync(ProductUpdateDto Dto);
}