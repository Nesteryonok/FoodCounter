using FoodCounter.Core.Entities;
using FoodCounter.Core.Requests;
using FoodCounter.Сore.Requests;

namespace FoodCounter.Application.Users;

public class DeleteUserCommand(
    IRepository<User> usersRepository
) : ICommand<ByIdRequest, BaseResponse>
{
    public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
    {
        var user = await usersRepository.GetOneAsync(
            u => u.Id.Value == request.Id,
            cancellationToken);

        if (user is null)
        {
            return new BaseResponse(404, "User not found.");
        }

        await usersRepository.RemoveAsync(user, cancellationToken);
        await usersRepository.SaveChangesAsync(cancellationToken);

        return new BaseResponse(200, "OK");
    }
}
