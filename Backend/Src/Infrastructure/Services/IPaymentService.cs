
namespace Backend.Src.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<Result<BasketDTO>> CreateOrUpdatePaymentIntent(int basketId);
        Task<Result<bool>> RemotePaymentWebhook(string jsonBody, string signature);
    }
}