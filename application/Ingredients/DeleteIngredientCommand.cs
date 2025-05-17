using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Ingredients;

public class DeleteIngredientCommand(
    IRepository<Ingredient> ingredientsRepository
) : ICommand<ByIdRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
    {
        var ingredient = await ingredientsRepository.GetOneAsync(
            i => i.Id.Value == request.Id,
            cancellationToken);

        if (ingredient is null)
            return new BaseResponse(404, "Ingredient not found.");

        await ingredientsRepository.RemoveAsync(ingredient, cancellationToken);
        await ingredientsRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse(200, "OK");
    }
}