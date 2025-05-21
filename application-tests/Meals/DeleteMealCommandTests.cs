using Bogus;
using FoodCounter.Core.Entities;
using FoodCounter.Core.Requests;
using FoodCounter.Core.Responses;
using FoodCounter.Tests.Application;
using FoodCounter.Сore.Requests;
using NUnit.Framework;

namespace FoodCounter.Tests.Application.Meals;

public class DeleteMealCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenMealExists_DeletesMealAndReturnsOK()
    {
        // Arrange
        var meal = new Meal
        {
            Id = new Id(Guid.NewGuid()),
            Name = "Test Meal"
        };
        var mealsRepository = new FakeRepository<Meal>(new[] { meal });
        var command = new DeleteMealCommand(mealsRepository);
        var request = new ByIdRequest(meal.Id.Value);

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(mealsRepository.Db, Is.Empty); 
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenMealNotFound_ReturnsNotFound()
    {
        // Arrange
        var mealsRepository = new FakeRepository<Meal>();
        var command = new DeleteMealCommand(mealsRepository);
        var request = new ByIdRequest(Guid.NewGuid()); 

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(404));
            Assert.That(response.Description, Is.EqualTo("Meal not found."));
            Assert.That(mealsRepository.Db, Is.Empty); 
        });
    }

    
}