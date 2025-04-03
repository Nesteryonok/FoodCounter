namespace FoodCounter.core.Entities;

public class MealIngredient : IEntity
{
    public Id Id { get; set; }
    public Meal Meal { get; set; }
    public Ingredient Ingredient { get; set; }
    public decimal Weight { get; set; }
}
