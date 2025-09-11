
namespace Backend.Src.Application.Interfaces;

public interface IBasketService
{
    Task<Result<BasketDto>> AddCouponAsync(BasketCouponDto dto);
    Task<Result<BasketDto>> AddItemAsync(BasketItemCreateDto dto);
    Task<Result<BasketDto>> CreateBasketAsync();
    Task<Result<BasketDto>> GetBasketAsync(int id);
    Task<Result<bool>> RemoveCouponAsync(BasketCouponDto dto);
    Task<Result<bool>> RemoveItemAsync(BasketItemDto dto);
}