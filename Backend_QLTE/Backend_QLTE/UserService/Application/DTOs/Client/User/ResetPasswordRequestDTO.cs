using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Client.User
{
    public class ResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "UserId là bắt buộc!")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
            ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự, có chữ hoa, số và ký tự đặc biệt [ @$!%*?&. ]!")]
        public string NewPassword { get; set; } // Mật khẩu đăng ký

        [Required(ErrorMessage = "Mật khẩu nhập lại không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmNewPassword { get; set; } // Mật khẩu đăng ký


    }
}
