using NUnit.Framework;


namespace FoodCounter.Tests.Application.Meals
{
    public class GetAllMealCommandTests
    {
        [Test]
        public async Task ExecuteAsync_WhenMealsExist_ReturnsAllMeals()
        {
            // Arrange
            var mealGenerator = new Faker<Meal>()
                .RuleFor(m => m.Id, f => new Id(f.Random.Guid()))
                .RuleFor(m => m.Name, f => f.Lorem.Word())
                .RuleFor(m => m.Ingredients, f => new List<MealIngredient>()); 

            var meals = mealGenerator.Generate(10).ToArray();
            var mealsRepository = new FakeRepository<Meal>(meals);
            var command = new GetAllMealCommand(mealsRepository);

            // Act
            var response = await command.ExecuteAsync(new EmptyRequest());
            var lastMeal = response.Meals.LastOrDefault();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(response.Description, Is.EqualTo("OK"));
                Assert.That(response.Meals.Count(), Is.EqualTo(10));
                Assert.That(lastMeal, Is.Not.Null);
                Assert.That(lastMeal!.Name, Is.EqualTo(mealsRepository.Db.Last().Name));
                Assert.That(lastMeal.Ingredients.Count, Is.EqualTo(mealsRepository.Db.Last().Ingredients.Count));
               
            });
        }

        [Test]
        public async Task ExecuteAsync_WhenNoMealsExist_ReturnsEmptyList()
        {
            // Arrange
            var mealsRepository = new FakeRepository<Meal>();
            var command = new GetAllMealCommand(mealsRepository);

            // Act
            var response = await command.ExecuteAsync(new EmptyRequest());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(response.Description, Is.EqualTo("OK"));
                Assert.That(response.Meals.Count(), Is.EqualTo(0));
            });
        }
    }
}
