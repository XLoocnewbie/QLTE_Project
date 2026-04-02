namespace Backend_QLTE.UserService.Application.Interfaces.Validators
{
    public interface IContextValidator<T, TContext> : IValidator<T>
    {
        Task ValidateAsync(T dto, TContext context, CancellationToken cancellationToken = default);
    }
}
