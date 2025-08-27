namespace WheelzyAssessment.Models;

public class Car
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int MakeId { get; set; }
    public int ModelId { get; set; }
    public int? SubModelId { get; set; }
    public string ZipCode { get; set; } = string.Empty;    
    public virtual Make Make { get; set; } = null!;
    public virtual Model Model { get; set; } = null!;
    public virtual SubModel? SubModel { get; set; }
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}