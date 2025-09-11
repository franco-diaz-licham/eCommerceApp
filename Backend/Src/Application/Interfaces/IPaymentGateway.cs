namespace Backend.Src.Application.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentIntentModel> CreateOrUpdateAsync(long amountMinorUnits, string currency, string? existingIntentId);
    Task<decimal> CalculateDiscountFromAmount(string couponId, decimal baseAmountDollars);
    Task<CouponInfoModel?> TryGetCouponByPromoCodeAsync(string code);
    PaymentEventModel ParseWebhook(string jsonBody, string signature);
}

// Port/Boundary models
public sealed record PaymentIntentModel(string IntentId, string? ClientSecret);
public sealed record CouponInfoModel(string RemoteId, string Name, string PromotionCodeId, string PromotionCode, long? AmountOff, decimal? PercentOff);
public sealed record PaymentEventModel(bool Success, string? IntentId = null, string? Status = null, long? AmountReceived = null, string? Error = null);