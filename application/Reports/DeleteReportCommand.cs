using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Reports;

public class DeleteReportCommand(
    IRepository<Report> reportsRepository
) : ICommand<ByIdRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
    {
        var report = await reportsRepository.GetOneAsync(
            r => r.Id.Value == request.Id,
            cancellationToken);

        if (report is null)
        {
            return new BaseResponse(404, "Report not found.");
        }

        await reportsRepository.RemoveAsync(report, cancellationToken);
        await reportsRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse(200, "OK");
    }
}