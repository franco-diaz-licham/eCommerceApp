
namespace Backend.Src.Application.Interfaces;

public interface IBasketService
{
    Task<Result<BasketDTO>> AddCouponAsync(BasketCouponDTO dto);
    Task<BasketDTO?> AddItemAsync(BasketItemCreateDTO dto);
    Task<BasketDTO?> CreateBasketAsync();
    Task<BasketDTO?> GetBasketAsync(int id);
    Task<Result<BasketDTO>> RemoveCouponAsync(BasketCouponDTO dto);
    Task<BasketDTO?> RemoveItemAsync(BasketItemCreateDTO dto);
}