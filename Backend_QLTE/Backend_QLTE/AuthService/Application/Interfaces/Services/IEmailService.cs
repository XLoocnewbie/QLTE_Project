namespace Backend_QLTE.AuthService.Application.Interfaces.Services
{
    public interface IEmailService 
    {
        // Gửi email bất đồng bộ
        Task SendAsync(string to, string subject, string body);
    }
}
