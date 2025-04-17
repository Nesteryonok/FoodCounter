namespace FoodCounter.Core.Entities;

public class User : IEntity
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public double Height { get; set; }

    public double Weight { get; set; }
    public DateTime Birthday { get; set; }
    public ActivityLvl ActivityLvl { get; set;}
    public Goal Goal { get; set; }
    public Sex Sex { get; set; }
    public virtual ICollection<Report> Reports { get; set; } = [];
}
