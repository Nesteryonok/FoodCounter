namespace FoodCounter.Core.Requests;

public record AddOrUpdateUserReqest(
    Guid? Id,
    string Name,
    string Email,
    double Height,
    double Weight,
    DateTime Birthday,
    ActivityLvl Activity,
    Goal Goal,
    Sex Sex
) : IRequest;