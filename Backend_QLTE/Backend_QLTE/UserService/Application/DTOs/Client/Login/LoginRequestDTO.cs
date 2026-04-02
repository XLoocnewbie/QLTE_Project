using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Client.Login
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
