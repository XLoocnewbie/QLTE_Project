namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class UserClaimsDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
