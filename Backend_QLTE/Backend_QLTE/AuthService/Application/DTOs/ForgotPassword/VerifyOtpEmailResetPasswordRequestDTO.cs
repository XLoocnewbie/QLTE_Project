using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.ForgotPassword
{
    public class VerifyOtpEmailResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc!")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Loại OTP là bắt buộc!")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
            ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự, có chữ hoa, số và ký tự đặc biệt [ @$!%*?&. ]!")]
        public string NewPassword { get; set; } // Mật khẩu đăng ký

        [Required(ErrorMessage = "Mật khẩu nhập lại không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmNewPassword { get; set; } // Mật khẩu đăng ký
    }

}
