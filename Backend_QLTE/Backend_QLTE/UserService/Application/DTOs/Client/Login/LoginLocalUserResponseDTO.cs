namespace Backend_QLTE.UserService.Application.DTOs.Client.Login
{
    public class LoginLocalUserResponseDTO
    {
        public string UserId { get; set; } // Id người dùng
        public string Email { get; set; } // Email
        public string UserName { get; set; } // Tên tài khoản
        public string Role { get; set; } // Vai trò của người dùng
    }
}
