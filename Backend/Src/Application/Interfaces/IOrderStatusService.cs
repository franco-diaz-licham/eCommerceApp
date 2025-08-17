namespace Backend.Src.Application.Interfaces;

public interface IOrderStatusService
{
    IQueryable<OrderStatusDTO> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<OrderStatusDTO>> GetAsync(int id);
}