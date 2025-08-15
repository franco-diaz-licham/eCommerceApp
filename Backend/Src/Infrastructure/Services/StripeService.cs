namespace Backend.Src.Infrastructure.Services;

public class StripeService : IPaymentService
{
    private string _stripeKey = default!;

    public StripeService(IConfiguration config)
    {
        _stripeKey = config["StripeSettings:SecretKey"] ?? "";
    }

    public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(BasketDTO basket, bool removeDiscount = false)
    {
        StripeConfiguration.ApiKey = _stripeKey;
        var service = new PaymentIntentService();
        var intent = new PaymentIntent();
        decimal subtotal = basket.Subtotal;
        decimal deliveryFee = subtotal > 10000 ? 0 : 500;
        decimal discount = 0;

        if (basket.Coupon != null) discount = await CalculateDiscountFromAmount(basket.Coupon, subtotal, removeDiscount);
        var total = subtotal - discount + deliveryFee;

        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)total,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            intent = await service.CreateAsync(options);
        }
        else
        {
            var options = new PaymentIntentUpdateOptions { Amount = (long)total };
            await service.UpdateAsync(basket.PaymentIntentId, options);
        }

        return intent;
    }

    public async Task<CouponDTO?> GetCouponFromPromoCode(string code)
    {
        var promoService = new PromotionCodeService();
        var options = new PromotionCodeListOptions { Code = code };
        var promoCodes = await promoService.ListAsync(options);
        var promoCode = promoCodes.FirstOrDefault();
        if (promoCode == null || promoCode.Coupon == null) return null;
        var output = new CouponDTO
        {
            Name = promoCode.Coupon.Name,
            AmountOff = promoCode.Coupon.AmountOff,
            PercentOff = promoCode.Coupon.PercentOff,
            CouponId = promoCode.Coupon.Id,
            PromotionCode = promoCode.Code
        };
        return output;
    }

    public async Task<decimal> CalculateDiscountFromAmount(CouponDTO appCoupon, decimal amount, bool removeDiscount = false)
    {
        var couponService = new CouponService();
        var coupon = await couponService.GetAsync(appCoupon.CouponId);
        if (coupon.AmountOff.HasValue && !removeDiscount) return (decimal)coupon.AmountOff;
        else if (coupon.PercentOff.HasValue && !removeDiscount) return (decimal)Math.Round(amount * (coupon.PercentOff.Value / 100), MidpointRounding.AwayFromZero);
        return 0;
    }
}
