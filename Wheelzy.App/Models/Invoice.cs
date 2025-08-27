namespace WheelzyAssessment.Models;

public class Invoice
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public decimal Total { get; set; }
    public DateTime InvoiceDate { get; set; }
    
    public virtual Customer? Customer { get; set; }
}