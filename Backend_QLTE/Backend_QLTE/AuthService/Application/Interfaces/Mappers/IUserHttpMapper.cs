using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.User;

namespace Backend_QLTE.AuthService.Application.Interfaces.Mappers
{
    public interface IUserHttpMapper
    {
        ResetPasswordEmailHttpRequestDTO ToResetPasswordDTO(VerifyOtpEmailResetPasswordRequestDTO request, string userId); // Chuyển đổi DTO yêu cầu xác minh OTP thành DTO yêu cầu đặt lại mật khẩu
        ChangeEmailHttpRequestDTO ToChangEmailDTO(VerifyChangeEmailRequestDTO request, string userId); // Chuyển đổi DTO yêu cầu đổi email thành DTO yêu cầu đổi email HTTP
    }
}
