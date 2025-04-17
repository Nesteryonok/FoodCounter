using System.Diagnostics;

namespace FoodCounter.Core.Requests;

public record AddOrUpdateUserReqest(
    Guid? Id,
    string Name,
    double Height,
    double Weight,
    DateTime Birthday,
    Activity Activity,
    Goal Goal,
    Sex Sex
) : IRequest;