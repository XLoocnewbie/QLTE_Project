using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Client.Login
{
    public class ExternalLoginUserInfoDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } // Email đăng ký
        public string? NameND { get; set; } // Số điện thoại đăng ký
        public string? AvatarND { get; set; } // Mật khẩu đăng ký
        [Required(ErrorMessage = "Bạn chưa có Id Goolge cấp")]
        public string AuthId { get; set; } // ID từ Google
        public string TypeLogin { get; set; } // Kiểu đăng nhập
    }
}
