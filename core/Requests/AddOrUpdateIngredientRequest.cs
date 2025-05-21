namespace FoodCounter.Core.Requests;

public record AddOrUpdateIngredientRequest(
    Guid? Id,
    string Name,
    double Protein,
    double Fat,
    double Carbs
) : IRequest;