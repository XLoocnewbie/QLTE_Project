using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;

namespace Backend_QLTE.AuthService.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultDTO<GenerateTokenUserDTO>> LoginUserGenerateTokenAsync(LoginUserPasswordRequestDTO request, CancellationToken cancellationToken = default);  // Đăng nhập và tạo token cho người dùng
        Task<ResultDTO<GenerateTokenUserDTO>> LoginExternalGenerateTokenAsync(ExternalAuthTokenRequestDTO requestExternal, CancellationToken cancellationToken = default); // Đăng nhập hoặc đăng ký bằng Google và tạo token cho người dùng
        Task<ResultDTO<GenerateTokenUserDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default); // Tạo mới token từ refresh token
        Task<ResultDTO> RevokeRefreshTokenAsync(RevokeRefreshTokenRequestDTO request, CancellationToken cancellationToken = default); // Thu hồi refresh token
        Task<ResultDTO> LogoutAsync(LogOutRequestDTO request, CancellationToken cancellationToken = default); // Đăng xuất người dùng
        Task<ResultDTO> GenerateAndSendOtpEmailAsync(ForgotPasswordEmailRequestDTO forgot, CancellationToken cancellationToken = default); // Gửi mã OTP đến email người dùng
        Task<ResultDTO> VerifyForgotPasswordOtpAsync(VerifyOtpEmailRequestDTO request, CancellationToken cancellationToken = default); // Xác thực mã OTP cho việc quên mật khẩu
        Task<ResultDTO> VerifyOtpForResetPasswordAsync(VerifyOtpEmailResetPasswordRequestDTO request, CancellationToken cancellationToken = default); // Xác thực mã OTP và đặt lại mật khẩu
        Task<ResultDTO> GenerateAndSendOtpChangeEmailAsync(ChangeEmailOtpRequestDTO request, CancellationToken cancellationToken = default); // Gửi mã OTP đến email mới khi đổi email
        Task<ResultDTO> VerifyOtpForChangeEmailAsync(VerifyChangeEmailRequestDTO request, CancellationToken cancellationToken = default); // Xác thực mã OTP và đổi email
        Task<ResultListDTO<List<RefreshTokenInfoDTO>>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
    }
}
