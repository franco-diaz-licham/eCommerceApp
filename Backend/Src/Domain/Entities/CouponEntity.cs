namespace Backend.Src.Domain.Entities;

public class CouponEntity : BaseEntity
{
    public required string Name { get; set; }
    public long? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public required string PromotionCode { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
