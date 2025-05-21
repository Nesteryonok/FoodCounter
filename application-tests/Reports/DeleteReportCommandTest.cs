using FoodCounter.Сore.Requests; 
using NUnit.Framework;
using FoodCounter.Core.Enums;

namespace FoodCounter.Tests.Application.Reports;

public class DeleteReportCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenReportExists_ReturnsOK()
    {
        // Arrange
        var userGenerator = new Faker<User>()
            .RuleFor(u => u.Id, f => new Id(f.Random.Guid()))
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Height, f => f.Random.Double(150, 200))
            .RuleFor(u => u.Weight, f => f.Random.Double(50, 150))
            .RuleFor(u => u.Birthday, f => f.Date.Past(50, DateTime.Now.AddYears(-18)))
            .RuleFor(u => u.ActivityLvl, f => f.PickRandom<ActivityLvl>())
            .RuleFor(u => u.Goal, f => f.PickRandom<Goal>())
            .RuleFor(u => u.Sex, f => f.PickRandom<Sex>())
            .RuleFor(u => u.Reports, f => new List<Report>());

        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var mealIngredientGenerator = new Faker<MealIngredient>()
            .RuleFor(mi => mi.Weight, f => f.Random.Double(10, 100))
            .RuleFor(mi => mi.Ingredient, f => ingredientGenerator.Generate());

        var mealGenerator = new Faker<Meal>()
            .RuleFor(m => m.Id, f => new Id(f.Random.Guid()))
            .RuleFor(m => m.Name, f => f.Lorem.Word())
            .RuleFor(m => m.Ingredients, f => mealIngredientGenerator.Generate(3).ToList());

        var reportGenerator = new Faker<Report>()
            .RuleFor(r => r.Id, f => new Id(f.Random.Guid()))
            .RuleFor(r => r.Date, f => f.Date.Past())
            .RuleFor(r => r.Weight, f => f.Random.Double(50, 150))
            .RuleFor(r => r.User, f => userGenerator.Generate())
            .RuleFor(r => r.Meal, f => mealGenerator.Generate());

        var report = reportGenerator.Generate();
        report.User.Reports.Add(report); 
        var reportRepository = new FakeRepository<Report>(new[] { report });
        var command = new DeleteReportCommand(reportRepository);
        var request = new ByIdRequest(report.Id.Value);

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(reportRepository.Db, Is.Empty);
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenReportDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var reportRepository = new FakeRepository<Report>();
        var command = new DeleteReportCommand(reportRepository);
        var request = new ByIdRequest(Guid.NewGuid());

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