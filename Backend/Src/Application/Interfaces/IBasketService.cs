
namespace Backend.Src.Application.Interfaces;

public interface IBasketService
{
    Task<Result<BasketDto>> AddCouponAsync(BasketCouponDto dto);
    /// <summary>
    /// Creates a new basket if basket does not exist. It adds the selected produt item to this basket.
    /// </summary>
    Task<Result<BasketDto>> AddItemAsync(BasketItemCreateDto dto);
    Task<Result<BasketDto>> CreateBasketAsync();
    Task<Result<bool>> DeleteBasketAsync(int id);
    Task<Result<BasketDto>> GetBasketAsync(int id);
    Task<Result<bool>> RemoveCouponAsync(BasketCouponDto dto);
    Task<Result<bool>> RemoveItemAsync(BasketItemDto dto);
}