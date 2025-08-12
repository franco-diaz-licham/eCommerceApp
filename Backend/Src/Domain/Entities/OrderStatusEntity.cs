namespace Backend.Src.Domain.Entities;

public class OrderStatusEntity : BaseEntity
{
    public required string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
