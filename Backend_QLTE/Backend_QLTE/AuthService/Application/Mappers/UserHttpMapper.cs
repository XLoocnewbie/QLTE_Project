using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Application.Interfaces.Mappers;

namespace Backend_QLTE.AuthService.Application.Mappers
{
    public class UserHttpMapper : IUserHttpMapper
    {
        public ResetPasswordEmailHttpRequestDTO ToResetPasswordDTO(VerifyOtpEmailResetPasswordRequestDTO request, string userId)
        {
            return new ResetPasswordEmailHttpRequestDTO
            {
                UserId = userId,
                NewPassword = request.NewPassword,
                ConfirmNewPassword = request.ConfirmNewPassword
            };
        }

        public ChangeEmailHttpRequestDTO ToChangEmailDTO(VerifyChangeEmailRequestDTO request, string userId)
        {
            return new ChangeEmailHttpRequestDTO
            {
                UserId = userId,
                NewEmail = request.NewEmail,
            };
        }
    }
}
