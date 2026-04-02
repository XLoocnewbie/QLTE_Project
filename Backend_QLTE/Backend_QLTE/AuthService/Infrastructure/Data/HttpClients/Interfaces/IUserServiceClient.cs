using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.User;

namespace Backend_QLTE.AuthService.Infrastructure.Data.HttpClients.Interfaces
{
    public interface IUserServiceClient
    {
        // Gọi Api
        Task<ApiResponse<LoginTokenResponseDTO?>> LoginUserAsync(LoginUserPasswordRequestDTO request ,CancellationToken cancellationToken = default); // lOGIN USER 
        Task<ApiResponse<LoginTokenResponseDTO>?> ProviderLoginUserAsync(ExternalAuthUserInfoDTO request, CancellationToken cancellationToken = default); // Login User Google
        Task<ApiResponse<ResultDTO<UserResponseDTO>>?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default); // Lấy thông tin user theo email
        Task<ApiResponse<ResultDTO>> ResetPasswordAsync(ResetPasswordEmailHttpRequestDTO requet, CancellationToken cancellationToken = default); // Reset mật khẩu
        Task<ApiResponse<ResultDTO<UserResponseDTO>>?> FindUserByUserIdAsync(string userId, CancellationToken cancellationToken = default); // Lấy thông tin user theo userId
        Task<ApiResponse<ResultDTO>> ChangeEmailAsync(ChangeEmailHttpRequestDTO requestDTO, CancellationToken cancellationToken = default); // Đổi email
    }
}
