using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.ForgotPassword
{
    public class VerifyOtpEmailRequestDTO
    {
        [Required(ErrorMessage ="Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc!")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Loại OTP là bắt buộc!")]
        public string Type { get; set; }
    }
}
