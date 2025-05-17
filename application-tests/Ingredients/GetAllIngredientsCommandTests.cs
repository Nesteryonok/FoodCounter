using NUnit.Framework;

namespace FoodCounter.Tests.Application.Ingredients;

public class GetAllIngredientsCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenIngredientsExist_ReturnsAllIngredients()
    {
        // Arrange
        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var ingredients = ingredientGenerator.Generate(10).ToArray(); 
        var ingredientRepository = new FakeRepository<Ingredient>(ingredients);
        var command = new GetAllIngredientsCommand(ingredientRepository);

        // Act
        var response = await command.ExecuteAsync(new EmptyRequest());
        var ingredient = response.Ingredients.LastOrDefault();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(response.Ingredients.Count(), Is.EqualTo(10));
            Assert.That(ingredient, Is.Not.Null);
            Assert.That(ingredient!.Name, Is.EqualTo(ingredientRepository.Db.Last().Name));
            Assert.That(ingredient!.Protein, Is.EqualTo(ingredientRepository.Db.Last().Protein));
            Assert.That(ingredient!.Fat, Is.EqualTo(ingredientRepository.Db.Last().Fat));
            Assert.That(ingredient!.Carbs, Is.EqualTo(ingredientRepository.Db.Last().Carbs));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenNoIngredientsExist_ReturnsEmptyList()
    {
        // Arrange
        var ingredientRepository = new FakeRepository<Ingredient>();
        var command = new GetAllIngredientsCommand(ingredientRepository);

        // Act
        var response = await command.ExecuteAsync(new EmptyRequest());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(response.Ingredients.Count(), Is.EqualTo(0));
        });
    }
}