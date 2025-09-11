namespace Backend.Src.Application.Interfaces;

public interface IOrderStatusService
{
    Task<Result<List<OrderStatusDto>>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<OrderStatusDto>> GetAsync(int id);
}