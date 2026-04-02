using Backend_QLTE.AuthService.Application.Interfaces.Factories;
using Backend_QLTE.AuthService.Application.Options;
using Backend_QLTE.AuthService.Domain.Entities;
using Backend_QLTE.AuthService.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace Backend_QLTE.AuthService.Application.Factories
{
    public class TokenFactory : ITokenFactory
    {

        public readonly IOptions<AuthOptions> _authOptions;

        public TokenFactory(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        public RefreshToken CreateRefreshToken (RefreshTokenString refreshToken,string userId)
        {
            return new RefreshToken
            {
                RFTokenId = Guid.NewGuid().ToString("N"),
                Token = refreshToken.Value,
                UserId = userId,
                ExpiredAt = DateTime.UtcNow.AddDays(_authOptions.Value.RefreshTokenExpireDays) // Mặc định mã refresh token hết hạn sau 7 ngày
            };
        }
    }
}
