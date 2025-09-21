namespace Backend.Src.Application.DTOs;

public class BasketCouponDto
{
    public int BasketId { get; set; }
    public required string PromotionCode { get; set; }
}
