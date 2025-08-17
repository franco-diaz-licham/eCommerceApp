namespace Backend.Src.Application.DTOs;

public class BasketDTO
{
    public int Id { get; set; }
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal Subtotal { get; set; }
    public int CouponId { get; set; }
    public CouponDTO? Coupon { get; set; }
    public List<BasketItemDTO> BasketItems { get; set; } = [];
}
