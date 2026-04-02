namespace Backend_QLTE.AuthService.Application.Interfaces.Exceptions
{
    public interface IHasHttpStatus
    {
        int StatusCode { get; }
    }
}
