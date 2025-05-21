using NUnit.Framework;
using FoodCounter.Core.Enums;

namespace FoodCounter.Tests.Application.Reports;

public class GetAllReportCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenReportsExist_ReturnsAllReports()
    {
        // Arrange
        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var mealIngredientGenerator = new Faker<MealIngredient>()
            .RuleFor(mi => mi.Weight, f => f.Random.Double(10, 100)) 
            .RuleFor(mi => mi.Ingredient, f => ingredientGenerator.Generate());

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

        var reports = reportGenerator.Generate(5).ToArray();
     
        foreach (var report in reports)
        {
            report.User.Reports.Add(report);
        }

        var reportRepository = new FakeRepository<Report>(reports);
        var command = new GetAllReportCommand(reportRepository);

        // Act
        var response = await command.ExecuteAsync(new EmptyRequest());
        var lastReport = response.Reports.LastOrDefault(); 

        var lastMeal = reportRepository.Db.Last().Meal;
        var totalWeight = lastMeal.Ingredients.Sum(x => x.Weight);
        var expectedProtein = lastMeal.Ingredients.Sum(x => x.Weight * x.Ingredient.Protein / 100) / totalWeight * 100;
        var expectedFat = lastMeal.Ingredients.Sum(x => x.Weight * x.Ingredient.Fat / 100) / totalWeight * 100;
        var expectedCarbs = lastMeal.Ingredients.Sum(x => x.Weight * x.Ingredient.Carbs / 100) / totalWeight * 100;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(response.Reports.Count(), Is.EqualTo(5));
            Assert.That(lastReport, Is.Not.Null); 
            Assert.That(lastReport!.Date, Is.EqualTo(reportRepository.Db.Last().Date));
            Assert.That(lastReport!.Weight, Is.EqualTo(reportRepository.Db.Last().Weight));
            Assert.That(lastReport!.User, Is.Not.Null);
            Assert.That(lastReport!.User.Name, Is.EqualTo(reportRepository.Db.Last().User.Name));
            Assert.That(lastReport!.User.Email, Is.EqualTo(reportRepository.Db.Last().User.Email));
            Assert.That(lastReport!.User.Height, Is.EqualTo(reportRepository.Db.Last().User.Height));
            Assert.That(lastReport!.User.Weight, Is.EqualTo(reportRepository.Db.Last().User.Weight));
            Assert.That(lastReport!.User.Birthday, Is.EqualTo(reportRepository.Db.Last().User.Birthday));
            Assert.That(lastReport!.User.ActivityLvl, Is.EqualTo(reportRepository.Db.Last().User.ActivityLvl));
            Assert.That(lastReport!.User.Goal, Is.EqualTo(reportRepository.Db.Last().User.Goal));
            Assert.That(lastReport!.User.Sex, Is.EqualTo(reportRepository.Db.Last().User.Sex));
            Assert.That(lastReport!.User.Reports, Has.Count.EqualTo(1)); 
            Assert.That(lastReport!.Meal, Is.Not.Null);
            Assert.That(lastReport!.Meal.Name, Is.EqualTo(reportRepository.Db.Last().Meal.Name));
            Assert.That(lastReport!.Meal.Protein, Is.EqualTo(expectedProtein).Within(0.001));
            Assert.That(lastReport!.Meal.Fat, Is.EqualTo(expectedFat).Within(0.001));
            Assert.That(lastReport!.Meal.Carbs, Is.EqualTo(expectedCarbs).Within(0.001));
            Assert.That(lastReport!.Meal.Ingredients, Has.Count.EqualTo(3));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenNoReportsExist_ReturnsEmptyList()
    {
        // Arrange
        var reportRepository = new FakeRepository<Report>();
        var command = new GetAllReportCommand(reportRepository);

        // Act
        var response = await command.ExecuteAsync(new EmptyRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(response.Reports.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ReturnsError()
    {
        // Arrange
        var reportRepository = new FakeRepository<Report>
        {
            SimulateException = true
        };
        var command = new GetAllReportCommand(reportRepository);

        // Act
        var response = await command.ExecuteAsync(new EmptyRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(500));
            Assert.That(response.Description, Is.EqualTo("Internal server error"));
            Assert.That(response.Reports.Count(), Is.EqualTo(0));
        });
    }
}