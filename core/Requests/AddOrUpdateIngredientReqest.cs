namespace FoodCounter.Core.Requests;

public record AddOrUpdateIngredientReqest(
    Guid? Id,
    string Name,
    double Protein,
    double Fat,
    double Carbs
) : IRequest;