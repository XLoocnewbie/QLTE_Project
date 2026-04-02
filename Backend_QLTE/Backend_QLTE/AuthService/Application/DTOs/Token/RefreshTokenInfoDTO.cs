using System;

namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class RefreshTokenInfoDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime? RevokedAt { get; set; }
    }
}
