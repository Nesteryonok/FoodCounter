namespace FoodCounter.Application.Ingredients;

public class GetAllIngredientsCommand(
    IRepository<Ingredient> ingredientsRepository
) : ICommand<EmptyRequest, IngredientsArrayResponse>
{
    public async Task<IngredientsArrayResponse> ExecuteAsync(EmptyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var ingredients = await ingredientsRepository.GetAllAsync(cancellationToken);
            return new IngredientsArrayResponse(200, "OK", ingredients);
        }
        catch (Exception)
        {
            return new IngredientsArrayResponse(500, "Internal server error", []);
        }
    }
}