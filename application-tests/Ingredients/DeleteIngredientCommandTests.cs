using FoodCounter.Сore.Requests; 
using NUnit.Framework;

namespace FoodCounter.Tests.Application.Ingredients;

public class DeleteIngredientCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenIngredientExists_ReturnsOK()
    {
        // Arrange
        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var ingredient = ingredientGenerator.Generate();
        var ingredientRepository = new FakeRepository<Ingredient>(new[] { ingredient });
        var command = new DeleteIngredientCommand(ingredientRepository);
        var request = new ByIdRequest(ingredient.Id.Value);

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(ingredientRepository.Db, Is.Empty); 
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenIngredientDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var ingredientRepository = new FakeRepository<Ingredient>();
        var command = new DeleteIngredientCommand(ingredientRepository);
        var request = new ByIdRequest(Guid.NewGuid()); 

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(404));
            Assert.That(response.Description, Is.EqualTo("Ingredient not found."));
            Assert.That(ingredientRepository.Db, Is.Empty); 
        });
    }
}