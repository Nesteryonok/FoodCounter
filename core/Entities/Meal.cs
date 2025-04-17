namespace FoodCounter.Core.Entities;

public class Meal : IFood
{
    public Id Id { get; set; }
    public string Name { get; set; }
    public double Protein => 
            Ingredients.Select(x => x.Weight * x.Ingredient.Protein / 100).Sum() /
            Ingredients.Select(x => x.Weight).Sum() * 100;
    public double Fat =>
            Ingredients.Select(x => x.Weight * x.Ingredient.Fat / 100).Sum() /
            Ingredients.Select(x => x.Weight).Sum() * 100;
    public double Carbs =>
            Ingredients.Select(x => x.Weight * x.Ingredient.Carbs / 100).Sum() /
            Ingredients.Select(x => x.Weight).Sum() * 100;
    public virtual ICollection<MealIngredient> Ingredients {get; set;}
}
