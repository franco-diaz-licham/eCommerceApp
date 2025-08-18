
namespace Backend.Src.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<BasketDto>> CreateOrUpdatePaymentIntent(int basketId);
    Task<Result<bool>> RemotePaymentWebhook(string jsonBody, string signature);
}