using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using FoodCounter.Application.Users;
using FoodCounter.Core.Entities;
using FoodCounter.Core.Enums;

namespace FoodCounter.Tests.Application.Users
{
    public class AddOrUpdateUserCommandTests
    {
        [TestCase(null, "Alice", "alice@example.com", 170, 65, 1990, ActivityLvl.Low, Goal.WeightMaintenance, Sex.Woman)]
        [TestCase(null, "Bob", "bob@example.com", 180, 80, 1985, ActivityLvl.Medium, Goal.MuscleGain, Sex.Man)]
        public async Task ExecuteAsync_WhenUserIsAdded_ReturnsOK(
            Guid? id,
            string name,
            string email,
            double height,
            double weight,
            int birthYear,
            ActivityLvl activity,
            Goal goal,
            Sex sex)
        {
            // Arrange
            var userRepository = new FakeRepository<User>();
            var command = new AddOrUpdateUserCommand(userRepository);

            var request = new AddOrUpdateUserReqest(
                id,
                name,
                email,
                height,
                weight,
                new DateTime(birthYear, 1, 1),
                activity,
                goal,
                sex);

            // Act
            var response = await command.ExecuteAsync(request);
            var addedUser = userRepository.Db.Last();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(userRepository.Db, Has.Count.EqualTo(1));
                Assert.That(addedUser.Name, Is.EqualTo(name));
                Assert.That(addedUser.Email, Is.EqualTo(email));
                Assert.That(addedUser.Height, Is.EqualTo(height));
                Assert.That(addedUser.Weight, Is.EqualTo(weight));
                Assert.That(addedUser.Birthday.Year, Is.EqualTo(birthYear));
                Assert.That(addedUser.ActivityLvl, Is.EqualTo(activity));
                Assert.That(addedUser.Goal, Is.EqualTo(goal));
                Assert.That(addedUser.Sex, Is.EqualTo(sex));
            });
        }

        [Test]
        public async Task ExecuteAsync_WhenUserIsUpdated_ReturnsOK()
        {
            // Arrange
            var id = Guid.Parse("a3bb8f1e-1208-430a-9e48-c221557ec962");
            var existingUser = new User
            {
                Id = new Id(id),
                Name = "Old Name",
                Email = "alice@example.com",
                Height = 160,
                Weight = 60,
                Birthday = new DateTime(1990, 1, 1),
                ActivityLvl = ActivityLvl.Low,
                Goal = Goal.WeightMaintenance,
                Sex = Sex.Woman
            };

            var userRepository = new FakeRepository<User>(new[] { existingUser });
            var command = new AddOrUpdateUserCommand(userRepository);

            var request = new AddOrUpdateUserReqest(
                id,
                "Updated Alice",
                "alice@example.com",
                172,
                67,
                new DateTime(1990, 1, 1),
                ActivityLvl.Medium,
                Goal.WeightLoss,
                Sex.Woman);

            // Act
            var response = await command.ExecuteAsync(request);
            var updatedUser = userRepository.Db.Last();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(userRepository.Db, Has.Count.EqualTo(1));
                Assert.That(updatedUser.Name, Is.EqualTo("Updated Alice"));
                Assert.That(updatedUser.Email, Is.EqualTo("alice@example.com"));
                Assert.That(updatedUser.Height, Is.EqualTo(172));
                Assert.That(updatedUser.Weight, Is.EqualTo(67));
                Assert.That(updatedUser.Birthday, Is.EqualTo(new DateTime(1990, 1, 1)));
                Assert.That(updatedUser.ActivityLvl, Is.EqualTo(ActivityLvl.Medium));
                Assert.That(updatedUser.Goal, Is.EqualTo(Goal.WeightLoss));
                Assert.That(updatedUser.Sex, Is.EqualTo(Sex.Woman));
            });
        }
    }
}
