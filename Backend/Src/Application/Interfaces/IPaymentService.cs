namespace Backend.Src.Application.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Creates a payment intent with the gateway.
    /// </summary>
    Task<Result<BasketDto>> CreateOrUpdatePaymentIntent(int basketId);

    /// <summary>
    /// Main payment processing when payment gateway sends request when payment has been received.
    /// </summary>
    Task<Result<bool>> RemotePaymentWebhook(string jsonBody, string signature);
}