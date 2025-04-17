using System.Diagnostics;
//нужно добавить email???

namespace FoodCounter.Application.Users
{
    public class AddOrUpdateUserCommand(
        IRepository<User> usersRepository
    ) : ICommand<AddOrUpdateUserReqest, BaseResponse>
    {
        public async Task<BaseResponse> ExecuteAsync(AddOrUpdateUserReqest addUserRequest,
                                                     CancellationToken cancellationToken = default)
        {
            bool isAdding = addUserRequest.Id is null;
            User? potentialUser = null;
            if (isAdding)
            {
                potentialUser = await usersRepository.GetOneAsync((user) => user.Name == addUserRequest.Name &&
                                                                           user.Birthday == addUserRequest.Birthday,
                                                                   cancellationToken);
                if (potentialUser is not null)
                    return new(409, $"User with name {addUserRequest.Name} already exists in the system.");
            }
            else
            {
                potentialUser = await usersRepository.GetOneAsync(x => x.Id.Value == addUserRequest.Id!.Value, cancellationToken);
                if (potentialUser is null)
                    return new(404, "No user found!");

                potentialUser.Name = addUserRequest.Name;
                potentialUser.Height = addUserRequest.Height;
                potentialUser.Weight = addUserRequest.Weight;
                potentialUser.Birthday = addUserRequest.Birthday;
                potentialUser.Activity = addUserRequest.Activity;
                potentialUser.Goal = addUserRequest.Goal;
                potentialUser.Sex = addUserRequest.Sex;
            }

            await usersRepository.AddOrUpdateAsync(potentialUser ?? new()
            {
                Name = addUserRequest.Name,
                Height = addUserRequest.Height,
                Weight = addUserRequest.Weight,
                Birthday = addUserRequest.Birthday,
                Activity = addUserRequest.Activity,
                Goal = addUserRequest.Goal,
                Sex = addUserRequest.Sex
            }, cancellationToken);

            return new(200, "OK");
        }
    }
}
