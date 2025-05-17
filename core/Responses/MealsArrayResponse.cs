namespace FoodCounter.Core.Responses;

public record MealsArrayResponse(
    int Code,
    string Description,
    IEnumerable<Meal> Meals
) : IResponse;