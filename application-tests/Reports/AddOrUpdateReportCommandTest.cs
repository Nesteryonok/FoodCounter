using FoodCounter.Сore.Requests; 
using NUnit.Framework;

namespace FoodCounter.Tests.Application.Reports;

public class AddOrUpdateReportCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenAddingNewReport_ReturnsOK()
    {
        // Arrange
        var reportRepository = new FakeRepository<Report>();
        var command = new AddOrUpdateReportCommand(reportRepository);
        var request = new AddOrUpdateReportRequest( 
            null, 
            DateTime.Now,
            75.5
        );

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(reportRepository.Db.Count, Is.EqualTo(1));
            Assert.That(reportRepository.Db[0].Date, Is.EqualTo(request.Date));
            Assert.That(reportRepository.Db[0].Weight, Is.EqualTo(request.Weight));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenUpdatingExistingReport_ReturnsOK()
    {
        // Arrange
        var existingReport = new Report
        {
            Id = new Id(Guid.NewGuid()),
            Date = DateTime.Now.AddDays(-1),
            Weight = 60.0
        };
        var reportRepository = new FakeRepository<Report>(new[] { existingReport });
        var command = new AddOrUpdateReportCommand(reportRepository);
        var request = new AddOrUpdateReportRequest( 
            existingReport.Id.Value,
            DateTime.Now,
            70.0
        );

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(reportRepository.Db.Count, Is.EqualTo(1));
            Assert.That(reportRepository.Db[0].Date, Is.EqualTo(request.Date)); 
            Assert.That(reportRepository.Db[0].Weight, Is.EqualTo(request.Weight));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenReportNotFound_ReturnsNotFound()
    {
        // Arrange
        var reportRepository = new FakeRepository<Report>();
        var command = new AddOrUpdateReportCommand(reportRepository);
        var request = new AddOrUpdateReportRequest( 
            Guid.NewGuid(), 
            DateTime.Now,
            75.5
        );

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(404));
            Assert.That(response.Description, Is.EqualTo("Report not found."));
            Assert.That(reportRepository.Db, Is.Empty);
        });
    }

}