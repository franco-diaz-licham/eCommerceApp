
namespace Backend.Src.Application.Interfaces;

public interface IBasketService
{
    Task<Result<BasketDTO>> AddCouponAsync(BasketCouponDTO dto);
    Task<Result<BasketDTO>> AddItemAsync(BasketItemCreateDTO dto);
    Task<Result<BasketDTO>> CreateBasketAsync();
    Task<Result<BasketDTO>> GetBasketAsync(int id);
    Task<Result<bool>> RemoveCouponAsync(BasketCouponDTO dto);
    Task<Result<bool>> RemoveItemAsync(BasketItemCreateDTO dto);
}