namespace WheelzyAssessment.Models;

public class SubModel
{
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public virtual Model Model { get; set; } = null!;
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}