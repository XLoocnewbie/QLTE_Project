namespace Backend_QLTE.AuthService.Application.DTOs.ForgotPassword
{
    public class ResetPasswordEmailRequestDTO
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
