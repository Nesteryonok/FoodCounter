namespace FoodCounter.Core.Responses;

public record IngredientsArrayResponse(
    int Code,
    string Description,
    IEnumerable<Ingredient> Ingredients
) : IResponse;