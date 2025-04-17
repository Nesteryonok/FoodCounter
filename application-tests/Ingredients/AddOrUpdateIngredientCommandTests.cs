using NUnit.Framework;

namespace FoodCounter.Tests.Application.Ingredients
{
    public class AddOrUpdateIngredientCommandTests
    {
        private const string ingredientId = "a3bb8f1e-1208-430a-9e48-c221557ec962";
        private const string ingredientName = "Carrot";

        
        [TestCase("Carrot", 0.9, 0.2, 10)]
        [TestCase("Potato", 2.0, 0.1, 15)]
        public async Task ExecuteAsync_WhenIngredientIsAdded_ReturnsOK(string Name, double Protein, double Fat, double Carbs)
        {
            // Arrange
            FakeRepository<Ingredient> ingredientRepository = new();
            AddOrUpdateIngredientCommand command = new(ingredientRepository);
            AddOrUpdateIngredientReqest addIngredientRequest = new(null, Name, Protein, Fat, Carbs);

            // Act
            var response = await command.ExecuteAsync(addIngredientRequest);
            var addedIngredient = ingredientRepository.Db.Last();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(ingredientRepository.Db, Has.Count.EqualTo(1));
                Assert.That(addedIngredient.Name, Is.EqualTo(Name));
                Assert.That(addedIngredient.Protein, Is.EqualTo(Protein));
                Assert.That(addedIngredient.Fat, Is.EqualTo(Fat));
                Assert.That(addedIngredient.Carbs, Is.EqualTo(Carbs));
            });
        }

        [TestCase(ingredientId, "Carrot", 1.0, 0.3, 12)]
        [TestCase(ingredientId, "Potato", 2.1, 0.2, 18)]
        public async Task ExecuteAsync_WhenIngredientIsUpdated_ReturnsOK(Guid Id, string Name, double Protein, double Fat, double Carbs)
        {
            // Arrange
            FakeRepository<Ingredient> ingredientRepository = new(
                [
                    new Ingredient
                    {
                        Id = new(Id),
                        Name = "Old Name",
                        Protein = 0.5,
                        Fat = 0.1,
                        Carbs = 5
                    }
                ]);
            AddOrUpdateIngredientCommand command = new(ingredientRepository);
            AddOrUpdateIngredientReqest addIngredientRequest = new(Id, Name, Protein, Fat, Carbs);

            // Act
            var response = await command.ExecuteAsync(addIngredientRequest);
            var updatedIngredient = ingredientRepository.Db.Last();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(ingredientRepository.Db, Has.Count.EqualTo(1));
                Assert.That(updatedIngredient.Name, Is.EqualTo(Name));
                Assert.That(updatedIngredient.Protein, Is.EqualTo(Protein));
                Assert.That(updatedIngredient.Fat, Is.EqualTo(Fat));
                Assert.That(updatedIngredient.Carbs, Is.EqualTo(Carbs));
            });
        }
    }
}
