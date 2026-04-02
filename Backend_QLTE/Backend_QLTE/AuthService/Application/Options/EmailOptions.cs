namespace Backend_QLTE.AuthService.Application.Options
{
    public class EmailOptions
    {
        public string Name { get; set; }      // Tên hiển thị (display name)
        public string Email { get; set; }     // Email gửi
        public string Host { get; set; }      // SMTP host
        public int Port { get; set; }         // SMTP port
        public bool SSL { get; set; }   // SSL bật/tắt
        public string Pass { get; set; }      // Mật khẩu (App Password)
    }
}
