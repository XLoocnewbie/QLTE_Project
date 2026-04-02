namespace Backend_QLTE.UserService.Application.Interfaces.Validators
{
    public interface IEntityValidator<T, TEntity> : IValidator<T>
    {
        Task<TEntity> ValidateAndGetAsync(T dto, CancellationToken cancellationToken = default);
    }
}
