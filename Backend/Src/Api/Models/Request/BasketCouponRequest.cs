namespace Backend.Src.Api.Models.Request;

public class BasketCouponRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public required string Code { get; set; }
}
