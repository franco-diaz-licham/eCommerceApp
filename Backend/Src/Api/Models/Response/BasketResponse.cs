namespace Backend.Src.Api.Models.Response;

public class BasketResponse
{
    public int Id { get; set; }
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal Subtotal { get; set; }
    public int? CouponId { get; set; }
    public CouponResponse? Coupon { get; set; }
    public List<BasketItemResponse> BasketItems { get; set; } = [];
}
