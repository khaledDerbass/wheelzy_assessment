namespace WheelzyAssessment.Models;

public class BuyerZipCode
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    
    public virtual Buyer Buyer { get; set; } = null!;
}
