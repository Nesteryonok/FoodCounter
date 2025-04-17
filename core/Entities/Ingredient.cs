namespace FoodCounter.Core.Entities;

public class Ingredient : IFood
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
}
