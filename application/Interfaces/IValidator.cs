namespace FoodCounter.Application.Interfaces;

public interface IValidator<TEntity> where TEntity : class, IEntity
{
    Task<bool> ValidateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> ValidateRemovalAsync(TEntity entity, CancellationToken cancellationToken = default);

}
