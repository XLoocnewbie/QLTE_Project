namespace Backend_QLTE.AuthService.Application.DTOs.Login
{
    public class LoginTokenResponseDTO
    {
        public bool Status { get; set; } // Trạng thái thành công hay thất bại
        public string Msg { get; set; } // Thông báo trả về cho người dùng
        public DataResponseLoginTokenDTO Data { get; set; }  // Token JWT trả về sau khi đăng nhập thành công
        public string? Token { get; set; } // Token JWT trả về sau khi đăng nhập thành công
    }

    public class  DataResponseLoginTokenDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
