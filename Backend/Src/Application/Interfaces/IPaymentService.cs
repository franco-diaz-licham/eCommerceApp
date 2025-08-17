namespace Backend.Src.Application.Interfaces;

public interface IRemotePaymentService
{
    /// <summary>
    /// Calculates discount based on coupon.
    /// </summary>
    Task<decimal> CalculateDiscountFromAmount(string remoteId, decimal amount, bool removeDiscount = false);

    /// <summary>
    /// Creates or updates payment intent record in payment service.
    /// </summary>
    Task<PaymentIntent> CreateOrUpdatePaymentIntent(BasketDTO basket, bool removeDiscount = false);
    
    /// <summary>
    /// Method which gets coupon based on code stored in payment storage.
    /// </summary>
    Task<CouponDTO?> GetCouponFromPromoCode(string code);
}