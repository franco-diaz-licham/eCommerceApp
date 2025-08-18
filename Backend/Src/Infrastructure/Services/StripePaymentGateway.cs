namespace Backend.Src.Infrastructure.Services;

public class StripePaymentGateway : IPaymentGateway
{
    private readonly string _stripeKey;
    private readonly IConfiguration _config;

    public StripePaymentGateway(IConfiguration config)
    {
        _stripeKey = config["StripeSettings:SecretKey"] ?? "";
        _config = config;
    }

    public async Task<PaymentIntentModel> CreateOrUpdateAsync(long amountMinorUnits, string currency, string? existingIntentId)
    {
        StripeConfiguration.ApiKey = _stripeKey;
        var service = new PaymentIntentService();

        PaymentIntent intent;
        if (string.IsNullOrWhiteSpace(existingIntentId))
        {
            intent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amountMinorUnits,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            });
        }
        else
        {
            intent = await service.UpdateAsync(existingIntentId, new PaymentIntentUpdateOptions
            {
                Amount = amountMinorUnits
            });
        }
        return new PaymentIntentModel(intent.Id, intent.ClientSecret);
    }

    public async Task<CouponInfoModel?> TryGetCouponByPromoCodeAsync(string code)
    {
        StripeConfiguration.ApiKey = _stripeKey;
        var promoService = new PromotionCodeService();
        var promos = await promoService.ListAsync(new PromotionCodeListOptions { Code = code });

        var promo = promos.FirstOrDefault();
        var c = promo?.Coupon;
        if (c is null) return null;

        return new CouponInfoModel(
            RemoteId: c.Id,
            Name: c.Name,
            AmountOff: c.AmountOff,
            PercentOff: c.PercentOff,
            PromotionCodeId: promo.Id,
            PromotionCode: promo.Code
        );
    }

    public async Task<decimal> CalculateDiscountFromAmount(string couponId, decimal baseAmountDollars)
    {
        StripeConfiguration.ApiKey = _stripeKey;
        var couponService = new CouponService();
        var coupon = await couponService.GetAsync(couponId);

        if (coupon.AmountOff.HasValue) return coupon.AmountOff.Value / 100m; // convert cents → dollars
        if (coupon.PercentOff.HasValue) return Math.Round(baseAmountDollars * (coupon.PercentOff.Value / 100m), MidpointRounding.AwayFromZero);
        return 0m;
    }

    public PaymentEventModel ParseWebhook(string jsonBody, string signature)
    {
        var stripeEvent = new Event();
        try
        {
            var webHookSecret = _config.GetValue<string>("StripeSettings:WhSecret") ?? "";
            stripeEvent = EventUtility.ConstructEvent(jsonBody, signature, webHookSecret);
        }
        catch (Exception ex)
        {
            return new PaymentEventModel(false, Error: ex.Message);
        }

        if (stripeEvent.Data.Object is not PaymentIntent intent) return new PaymentEventModel(false);
        return new PaymentEventModel(true, intent.Id, intent.Status, intent.AmountReceived);
    }
}