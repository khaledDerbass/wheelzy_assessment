namespace WheelzyAssessment.Models;

public class Model
{
    public int ModelId { get; set; }
    public int MakeId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public virtual Make Make { get; set; } = null!;
    public virtual ICollection<SubModel> SubModels { get; set; } = new List<SubModel>();
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
