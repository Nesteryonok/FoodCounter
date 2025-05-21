using FoodCounter.Сore.Requests; 

namespace FoodCounter.Application.Reports;

public class AddOrUpdateReportCommand(
    IRepository<Report> reportsRepository
) : ICommand<AddOrUpdateReportRequest, BaseResponse> 
{
    public async Task<BaseResponse> ExecuteAsync(AddOrUpdateReportRequest request, CancellationToken cancellationToken = default)
    {
        bool isAdding = request.Id is null;
        Report? report;

        if (isAdding)
        {
            report = new Report();
        }
        else
        {
            report = await reportsRepository.GetOneAsync(
                x => x.Id.Value == request.Id!.Value,
                cancellationToken);

            if (report is null)
                return new BaseResponse(404, "Report not found.");
        }
        report.Date = request.Date; 
        report.Weight = request.Weight;
        await reportsRepository.AddOrUpdateAsync(report, cancellationToken);
        await reportsRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse(200, "OK");
    }
}