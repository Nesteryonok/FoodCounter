using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Meals;

public class AddOrUpdateMealCommand(
    IRepository<Meal> mealsRepository,
    IRepository<Ingredient> ingredientsRepository
) : ICommand<AddOrUpdateMealRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(AddOrUpdateMealRequest addMealRequest,
                                                 CancellationToken cancellationToken = default)
    {
        Meal? potentialMeal = null;

        if (addMealRequest.Id is not null)
        {
            potentialMeal = await mealsRepository.GetOneAsync(meal => meal.Id.Value == addMealRequest.Id!.Value,
                                                               cancellationToken);
            if (potentialMeal is null)
                return new(404, "No meal found for updating.");
        }
        else
            potentialMeal = new();
        potentialMeal.Name = addMealRequest.Name;

        var ingredients = new List<MealIngredient>();
        foreach (var ingredientDTO in addMealRequest.Ingredients)
        {
            var ingredient = await ingredientsRepository.GetOneAsync(i => i.Id.Value == ingredientDTO.Id, cancellationToken);
            if (ingredient is null)
                return new(404, $"Ingredient with Id {ingredientDTO.Id} not found.");

            ingredients.Add(new MealIngredient
            {
                Ingredient = ingredient,
                Weight = ingredientDTO.Weight,
                Meal = potentialMeal
            });
        }
        potentialMeal.Ingredients = ingredients;

        await mealsRepository.AddOrUpdateAsync(potentialMeal, cancellationToken);

        return new(200, "OK");
    }
}
