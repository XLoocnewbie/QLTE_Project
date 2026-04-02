using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.ForgotPassword
{
    public class ForgotPasswordEmailRequestDTO
    {
        [Required(ErrorMessage = "Email bắt buộc phải nhập!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Type bắt buộc phải nhập!")]
        public string Type { get; set; } // Loại yêu cầu, ví dụ: "ForgotEmail", "VerifyEmail"
    }
}
