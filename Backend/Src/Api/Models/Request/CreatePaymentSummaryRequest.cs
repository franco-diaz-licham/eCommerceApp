namespace Backend.Src.Api.Models.Request;

public class CreatePaymentSummaryRequest
{
    [Required] public int Last4 { get; set; }
    [Required] public required string Brand { get; set; }
    [Required] public int ExpMonth { get; set; }
    [Required] public int ExpYear { get; set; }
}
