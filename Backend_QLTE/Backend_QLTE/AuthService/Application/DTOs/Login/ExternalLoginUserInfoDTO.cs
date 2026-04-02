namespace Backend_QLTE.AuthService.Application.DTOs.Login
{
    public class ExternalLoginUserInfoDTO
    {
        public string AuthId { get; set; }   // Google.Subject / Facebook.id
        public string Email { get; set; }
        public string NameND { get; set; }
        public string AvatarND { get; set; }
        public string Provider { get; set; } // "Google", "Facebook"...
    }
}
