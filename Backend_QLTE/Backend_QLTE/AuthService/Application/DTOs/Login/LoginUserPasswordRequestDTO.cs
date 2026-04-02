using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.Login
{
    public class LoginUserPasswordRequestDTO
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
