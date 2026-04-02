namespace Backend_QLTE.AuthService.Application.Options
{
    public class AuthOptions
    {
        public JwtOptions Jwt { get; set; }
        public int RefreshTokenExpireDays { get; set; }
    }
}
