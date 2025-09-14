namespace Backend.Src.Api.Models.Response;

public class CouponResponse
{
    public int Id { get; set; }
    public required string PromotionCode { get; set; }
    public decimal? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
}
