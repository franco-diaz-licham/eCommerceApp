namespace Backend.Src.Application.DTOs;

public class CouponDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public long? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public required string PromotionCode { get; set; }
    public required string CouponId { get; set; }
}
