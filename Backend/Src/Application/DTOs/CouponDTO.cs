namespace Backend.Src.Application.DTOs;

public class CouponDTO
{
    public int Id { get; set; }
    public string RemoteId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public required string PromotionCode { get; set; }
}
