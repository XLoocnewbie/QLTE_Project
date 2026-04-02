namespace Backend_QLTE.AuthService.Application.DTOs.Login
{
    // backend nhận thông tin user sau khi verify với Google/Facebook.
    public class ExternalAuthUserInfoDTO
    {
        public string Email { get; set; } // Email đăng ký
        public string? NameND { get; set; } // Số điện thoại đăng ký
        public string? AvatarND { get; set; } // Mật khẩu đăng ký
        public string AuthId { get; set; } // ID từ Google
        public string TypeLogin { get; set; }  // "Google" | "Facebook"
    }
}
