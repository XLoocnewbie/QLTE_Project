namespace Backend_QLTE.AuthService.Application.Interfaces.Validators
{
    public interface IValidator<T>
    {
        Task ValidateAsync(T dto, CancellationToken cancellationToken = default);
    }
}
