using FoodCounter.Core.Entities;
using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Reports;
public class AddOrUpdateReportCommand(
    IRepository<Report> reportsRepository
   ) : ICommand<AddOrUpdateReportReqest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(AddOrUpdateReportReqest request, CancellationToken cancellationToken = default)
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
                return new(404, "Repurt not found.");
        }
        report.Date = request.Data;
        report.Weight = request.Weight;
        await reportsRepository.AddOrUpdateAsync(report, cancellationToken);
        await reportsRepository.SaveChangesAsync(cancellationToken);

        return new(200, "OK");
    }
}
