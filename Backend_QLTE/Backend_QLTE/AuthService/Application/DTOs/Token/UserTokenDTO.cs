namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class UserTokenDTO
    {
        public string UserId { get; set; } // Unique identifier for the user
        public string UserName { get; set; } // Username of the user
        public string Email { get; set; } // Email of the user
        public string Role { get; set; } // Role user
    }
}
