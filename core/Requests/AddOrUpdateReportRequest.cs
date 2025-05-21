namespace FoodCounter.Сore.Requests;

public record AddOrUpdateReportRequest(
    Guid? Id,
    DateTime Date,
    double Weight
    ):IRequest;

