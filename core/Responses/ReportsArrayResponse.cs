namespace FoodCounter.Core.Responses;

public record ReportsArrayResponse(
    int Code,
    string Description,
    IEnumerable<Report> Reports
) : IResponse;