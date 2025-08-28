namespace WheelzyAssessment.Models;

public class Make
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}