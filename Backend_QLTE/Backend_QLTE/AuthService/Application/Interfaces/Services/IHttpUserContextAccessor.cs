namespace Backend_QLTE.AuthService.Application.Interfaces.Services
{
    public interface IHttpUserContextAccessor
    {
        string? GetUserId(); // Lấy userId từ token
        string? GetEmail();
    }

}
