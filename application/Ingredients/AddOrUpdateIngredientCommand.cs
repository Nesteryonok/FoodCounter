namespace FoodCounter.Application.Ingredients;

public class AddOrUpdateIngredientCommand(
    IRepository<Ingredient> ingredientsRepository
) : ICommand<AddOrUpdateIngredientRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(AddOrUpdateIngredientRequest request, CancellationToken cancellationToken = default)
    {
        bool isAdding = request.Id is null;
        Ingredient? ingredient;

        if (isAdding)
        {
            ingredient = new Ingredient();
        }
        else
        {
            ingredient = await ingredientsRepository.GetOneAsync(
                x => x.Id.Value == request.Id!.Value,
                cancellationToken);

            if (ingredient is null)
                return new(404, "Ingredient not found.");
        }

        ingredient.Name = request.Name;
        ingredient.Protein = request.Protein;
        ingredient.Fat = request.Fat;
        ingredient.Carbs = request.Carbs;

        await ingredientsRepository.AddOrUpdateAsync(ingredient, cancellationToken);
        await ingredientsRepository.SaveChangesAsync(cancellationToken);

        return new(200, "OK");
    }
}