namespace FoodCounter.core.Entities;

public class Meal : IFood
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    public virtual ICollection<MealIngredient> Ingredients {get; set;}
}
