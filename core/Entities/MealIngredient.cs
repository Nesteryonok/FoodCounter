namespace FoodCounter.Core.Entities;

public class MealIngredient : IEntity
{
    public Id Id { get; set; }
    public Meal Meal { get; set; }
    public Ingredient Ingredient { get; set; }
    public double Weight { get; set; }
}
