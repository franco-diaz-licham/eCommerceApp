namespace Backend.Src.Domain.Entities;

public class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    public void Updated(DateTime date) => UpdatedOn = date;

    public void Created(DateTime date) => CreatedOn = date;
}
