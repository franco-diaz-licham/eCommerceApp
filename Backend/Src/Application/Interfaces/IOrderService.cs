namespace Backend.Src.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto);
    Task<Result<List<OrderDto>>> GetAllAsync(BaseQuerySpecs specs);
    Task<OrderDto?> GetAsync(int id, string email);
}