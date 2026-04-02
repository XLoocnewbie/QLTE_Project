namespace Backend_QLTE.AuthService.Application.Interfaces.Validators
{
    public interface IContextValidator<T, TContext> : IValidator<T>
    {
        Task ValidateAsync(T dto, TContext context, CancellationToken cancellationToken = default);
    }
}
