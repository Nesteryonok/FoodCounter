using FoodCounter.Core.Enums;
using NUnit.Framework;
using FoodCounter.Сore.Requests;

namespace FoodCounter.Tests.Application.Users
{
    public class DeleteUserCommandTests
    {
        [Test]
        public async Task ExecuteAsync_WhenUserExists_ReturnsOK()
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

            var user = userGenerator.Generate();
            var userRepository = new FakeRepository<User>(new[] { user });
            var command = new DeleteUserCommand(userRepository);
            var request = new ByIdRequest(user.Id.Value);

            // Act
            var response = await command.ExecuteAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(200));
                Assert.That(response.Description, Is.EqualTo("OK"));
                Assert.That(userRepository.Db, Is.Empty);
            });
        }

        [Test]
        public async Task ExecuteAsync_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userRepository = new FakeRepository<User>();
            var command = new DeleteUserCommand(userRepository);
            var request = new ByIdRequest(Guid.NewGuid());

            // Act
            var response = await command.ExecuteAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(404));
                Assert.That(response.Description, Is.EqualTo("User not found."));
                Assert.That(userRepository.Db, Is.Empty);
            });
        }
    }
}
