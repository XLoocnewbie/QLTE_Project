using Backend_QLTE.AuthService.Application.DTOs.Login;

namespace Backend_QLTE.AuthService.Application.Interfaces.Services
{
    public interface IExternalAuthProvider
    {
        string ProviderName { get; } // "google", "facebook"
        Task<ExternalLoginUserInfoDTO> ValidateAsync(string token);
    }
}
