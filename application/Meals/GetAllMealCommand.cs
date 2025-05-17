namespace FoodCounter.Application.Meals;

public class GetAllMealCommand(
    IRepository<Meal> mealsRepository
) : ICommand<EmptyRequest, MealsArrayResponse>
{
    public async Task<MealsArrayResponse> ExecuteAsync(EmptyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var meals = await mealsRepository.GetAllAsync(cancellationToken);
            return new MealsArrayResponse(200, "OK", meals);
        }
        catch (Exception)
        {
            return new MealsArrayResponse(500, "Internal server error", []);
        }
    }
}