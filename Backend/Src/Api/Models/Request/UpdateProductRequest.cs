namespace Backend.Src.Api.Models.Request;

public class UpdateProductRequest : CreateProductRequest
{
    [Required] public int Id { get; set; }
}
