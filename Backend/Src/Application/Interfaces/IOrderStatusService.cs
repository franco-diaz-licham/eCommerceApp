namespace Backend.Src.Application.Interfaces;

public interface IOrderStatusService
{
    Task<PagedList<OrderStatusDto>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<OrderStatusDto>> GetAsync(int id);
}