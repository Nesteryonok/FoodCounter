using System.Data;

namespace FoodCounter.Сore.Requests
{
    public record AddOrUpdateReportReqest(
        Guid? Id,
        DateTime Data,
        double Weight
        ):IRequest;
}
