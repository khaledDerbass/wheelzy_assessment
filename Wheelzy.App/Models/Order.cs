namespace WheelzyAssessment.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public int StatusId { get; set; }
    public decimal Total { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual Customer Customer { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
}