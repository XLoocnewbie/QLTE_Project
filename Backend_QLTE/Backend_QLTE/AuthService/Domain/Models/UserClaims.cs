namespace Backend_QLTE.AuthService.Domain.Models
{
    public class UserClaims
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public UserClaims(string userId, string userName, string email, string role)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            Role = role;
        }
    }
}
