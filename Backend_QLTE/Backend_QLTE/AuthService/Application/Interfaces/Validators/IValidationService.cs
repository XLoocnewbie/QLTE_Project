namespace Backend_QLTE.AuthService.Application.Interfaces.Validators
{
    public interface IValidationService
    {
        Task ValidateAsync<T>(T dto, CancellationToken cancellationToken = default);
        Task ValidateAsync<T, TContext>(T dto, TContext context, CancellationToken cancellationToken = default);
        Task<TEntity> ValidateAsync<T, TEntity>(T dto, CancellationToken cancellationToken = default);
    }
}
