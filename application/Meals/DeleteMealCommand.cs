using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Meals;

public class DeleteMealCommand(
    IRepository<Meal> mealsRepository
) : ICommand<ByIdRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
    {
        var meal = await mealsRepository.GetOneAsync(
            m => m.Id.Value == request.Id,
            cancellationToken);

        if (meal is null)
        {
            return new BaseResponse(404, "Meal not found.");
        }

        await mealsRepository.RemoveAsync(meal, cancellationToken);
        await mealsRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse(200, "OK");
    }
}
