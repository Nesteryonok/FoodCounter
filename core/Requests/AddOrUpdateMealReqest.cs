namespace FoodCounter.Сore.Requests;


public record AddOrUpdateMealReqest(
    Guid? Id,
    string Name,
    IngredientWeightDTO[] Ingredients
    ) : IRequest;

public record IngredientWeightDTO(Guid Id, double Weight);