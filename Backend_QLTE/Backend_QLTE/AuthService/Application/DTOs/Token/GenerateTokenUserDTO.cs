namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class GenerateTokenUserDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
