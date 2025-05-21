using FoodCounter.Сore.Requests;
using NUnit.Framework;

namespace FoodCounter.Tests.Application.Meals;

public class AddOrUpdateMealCommandTests
{
    [Test]
    public async Task ExecuteAsync_WhenAddingNewMeal_ReturnsOK()
    {
        // Arrange
        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var ingredients = ingredientGenerator.Generate(2);
        var mealsRepository = new FakeRepository<Meal>();
        var ingredientsRepository = new FakeRepository<Ingredient>(ingredients.ToArray());
        var command = new AddOrUpdateMealCommand(mealsRepository, ingredientsRepository);
        var request = new AddOrUpdateMealRequest(
            null, 
            "Test Meal",
            new IngredientWeightDTO[]
            {
                new(ingredients[0].Id.Value, 50.0),
                new(ingredients[1].Id.Value, 30.0)
            }
        );

        // Act
        var response = await command.ExecuteAsync(request);
        var addedMeal = mealsRepository.Db.First();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(mealsRepository.Db.Count, Is.EqualTo(1));
            Assert.That(addedMeal.Name, Is.EqualTo(request.Name));
            Assert.That(addedMeal.Ingredients.Count, Is.EqualTo(2));
            var ingredientList = addedMeal.Ingredients.ToList(); 
            Assert.That(ingredientList[0].Weight, Is.EqualTo(50.0));
            Assert.That(ingredientList[1].Weight, Is.EqualTo(30.0));
            Assert.That(ingredientList[0].Ingredient, Is.SameAs(ingredients[0]));
            Assert.That(ingredientList[1].Ingredient, Is.SameAs(ingredients[1]));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenUpdatingExistingMeal_ReturnsOK()
    {
        // Arrange
        var ingredientGenerator = new Faker<Ingredient>()
            .RuleFor(i => i.Id, f => new Id(f.Random.Guid()))
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.Protein, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Fat, f => f.Random.Double(0, 100))
            .RuleFor(i => i.Carbs, f => f.Random.Double(0, 100));

        var initialIngredients = ingredientGenerator.Generate(1);
        var newIngredients = ingredientGenerator.Generate(2);
        var existingMeal = new Meal
        {
            Id = new Id(Guid.NewGuid()),
            Name = "Old Meal",
            Ingredients = new List<MealIngredient>
            {
                new() { Ingredient = initialIngredients[0], Weight = 20.0 }
            }
        };
        existingMeal.Ingredients.First().Meal = existingMeal;

        var mealsRepository = new FakeRepository<Meal>(new[] { existingMeal });
        var ingredientsRepository = new FakeRepository<Ingredient>(newIngredients.ToArray());
        var command = new AddOrUpdateMealCommand(mealsRepository, ingredientsRepository);
        var request = new AddOrUpdateMealRequest(
            existingMeal.Id.Value,
            "Updated Meal",
            new IngredientWeightDTO[]
            {
                new(newIngredients[0].Id.Value, 60.0),
                new(newIngredients[1].Id.Value, 40.0)
            }
        );

        // Act
        var response = await command.ExecuteAsync(request);
        var updatedMeal = mealsRepository.Db.First();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.Description, Is.EqualTo("OK"));
            Assert.That(mealsRepository.Db.Count, Is.EqualTo(1));
            Assert.That(updatedMeal.Name, Is.EqualTo(request.Name));
            Assert.That(updatedMeal.Ingredients.Count, Is.EqualTo(2));
            var ingredientList = updatedMeal.Ingredients.ToList(); 
            Assert.That(ingredientList[0].Weight, Is.EqualTo(60.0));
            Assert.That(ingredientList[1].Weight, Is.EqualTo(40.0));
            Assert.That(ingredientList[0].Ingredient, Is.SameAs(newIngredients[0]));
            Assert.That(ingredientList[1].Ingredient, Is.SameAs(newIngredients[1]));
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenMealNotFound_ReturnsNotFound()
    {
        // Arrange
        var ingredientsRepository = new FakeRepository<Ingredient>();
        var mealsRepository = new FakeRepository<Meal>();
        var command = new AddOrUpdateMealCommand(mealsRepository, ingredientsRepository);
        var request = new AddOrUpdateMealRequest(
            Guid.NewGuid(), 
            "Test Meal",
            Array.Empty<IngredientWeightDTO>()
        );

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(404));
            Assert.That(response.Description, Is.EqualTo("No meal found for updating."));
            Assert.That(mealsRepository.Db, Is.Empty);
        });
    }

    [Test]
    public async Task ExecuteAsync_WhenIngredientNotFound_ReturnsNotFound()
    {
        // Arrange
        var mealsRepository = new FakeRepository<Meal>();
        var ingredientsRepository = new FakeRepository<Ingredient>();
        var command = new AddOrUpdateMealCommand(mealsRepository, ingredientsRepository);
        var request = new AddOrUpdateMealRequest(
            null,
            "Test Meal",
            new IngredientWeightDTO[]
            {
                new(Guid.NewGuid(), 50.0) 
            }
        );

        // Act
        var response = await command.ExecuteAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(404));
            Assert.That(response.Description, Is.EqualTo($"Ingredient with Id {request.Ingredients[0].Id} not found."));
            Assert.That(mealsRepository.Db, Is.Empty);
        });
    }

    

    [Test]
    public async Task ExecuteAsync_WhenAddingNewMeal_CalculatesNutritionalValuesCorrectly()
    {
        // Arrange
        var ingredients = new List<Ingredient>
        {
            new() { Id = new Id(Guid.NewGuid()), Name = "Chicken", Protein = 25.0, Fat = 5.0, Carbs = 0.0 },
            new() { Id = new Id(Guid.NewGuid()), Name = "Rice", Protein = 5.0, Fat = 1.0, Carbs = 30.0 }
        };
        var mealsRepository = new FakeRepository<Meal>();
        var ingredientsRepository = new FakeRepository<Ingredient>(ingredients.ToArray());
        var command = new AddOrUpdateMealCommand(mealsRepository, ingredientsRepository);
        var request = new AddOrUpdateMealRequest(
            null,
            "Chicken Rice",
            new IngredientWeightDTO[]
            {
                new(ingredients[0].Id.Value, 100.0), // 100 г курицы
                new(ingredients[1].Id.Value, 50.0)   // 50 г риса
            }
        );

        // Act
        var response = await command.ExecuteAsync(request);
        var addedMeal = mealsRepository.Db.First();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(addedMeal.Ingredients.Count, Is.EqualTo(2));
            Assert.That(addedMeal.Protein, Is.EqualTo(18.33).Within(0.01));
            Assert.That(addedMeal.Fat, Is.EqualTo(3.67).Within(0.01));
            Assert.That(addedMeal.Carbs, Is.EqualTo(10.0).Within(0.01));
        });
    }
}