namespace FoodCounter.Сore.Requests;


public record AddOrUpdateMealRequest(
    Guid? Id,
    string Name,
    IngredientWeightDTO[] Ingredients
    ) : IRequest;

public record IngredientWeightDTO(Guid Id, double Weight);