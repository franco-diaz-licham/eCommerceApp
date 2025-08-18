namespace Backend.Src.Application.DTOs;

public class BasketDto
{
    public int Id { get; set; }
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal Subtotal { get; set; }
    public int CouponId { get; set; }
    public CouponDto? Coupon { get; set; }
    public List<BasketItemDto> BasketItems { get; set; } = [];
}
