namespace WheelzyAssessment.Models;

public class Buyer
{
    public int BuyerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    public virtual ICollection<BuyerZipCode> CoverageAreas { get; set; } = new List<BuyerZipCode>();
}