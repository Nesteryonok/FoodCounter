namespace FoodCounter.Core.Entities;

public class Report : IEntity
{
    public Id Id { get; set; }
    public DateTime Date { get; set; }
    public double Weight { get; set; }

    public virtual User User { get; set; }
    public virtual Meal Meal { get; set; } 
}
