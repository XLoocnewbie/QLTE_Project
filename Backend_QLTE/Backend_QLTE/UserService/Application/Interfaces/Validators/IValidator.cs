namespace Backend_QLTE.UserService.Application.Interfaces.Validators
{
    public interface IValidator<T>
    {
        Task ValidateAsync(T dto, CancellationToken cancellationToken = default);
    }
}
