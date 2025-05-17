namespace FoodCounter.Application.Reports;

public class GetAllReportCommand(
    IRepository<Report> reportsRepository
) : ICommand<EmptyRequest, ReportsArrayResponse>
{
    public async Task<ReportsArrayResponse> ExecuteAsync(EmptyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var reports = await reportsRepository.GetAllAsync(cancellationToken);
            return new ReportsArrayResponse(200, "OK", reports);
        }
        catch (Exception)
        {
            return new ReportsArrayResponse(500, "Internal server error", []);
        }
    }
}