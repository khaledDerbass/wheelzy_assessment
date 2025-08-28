namespace WheelzyAssessment.Models;

public class CarStatusHistory
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int StatusId { get; set; }
    public bool IsCurrent { get; set; } = false;
    public DateTime? StatusDate { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
    
    public virtual Car Car { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
}