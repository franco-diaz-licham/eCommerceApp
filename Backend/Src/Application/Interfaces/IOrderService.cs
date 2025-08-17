namespace Backend.Src.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderDTO>> CreateOrderAsync(OrderCreateDTO dto);
        IQueryable<OrderDTO> GetAllAsync(BaseQuerySpecs specs);
        Task<OrderDTO?> GetAsync(int id, string email);
    }
}