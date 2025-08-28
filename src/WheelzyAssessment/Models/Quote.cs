namespace WheelzyAssessment.Models;

public class Quote
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int BuyerId { get; set; }
    public decimal Amount { get; set; }
    public bool IsCurrent { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public virtual Car Car { get; set; } = null!;
    public virtual Buyer Buyer { get; set; } = null!;
}