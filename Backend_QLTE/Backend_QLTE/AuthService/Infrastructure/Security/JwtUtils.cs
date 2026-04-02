using Backend_QLTE.AuthService.Application.Options;
using Backend_QLTE.AuthService.Domain.Interfaces.Jwt;
using Backend_QLTE.AuthService.Domain.Models;
using Backend_QLTE.AuthService.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_QLTE.AuthService.Infrastructure.Security
{
    public class JwtUtils : IJwtProvider
    {
        private readonly IOptions<AuthOptions> _settings;
        public JwtUtils(IOptions<AuthOptions> settings)
        {
            _settings = settings;
        }
        // Tạo token
        public Token GenerateToken(UserClaims userClaims)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userClaims.UserId),
                new Claim(ClaimTypes.Name, userClaims.UserName),
                new Claim(ClaimTypes.Email, userClaims.Email),
                new Claim(ClaimTypes.Role, userClaims.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(_settings.Value.Jwt.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Value.Jwt.Issuer,
                audience: _settings.Value.Jwt.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new Token(tokenString, expiry);
        }

        public bool ValidateToken(string token)
        {
            var principal = GetPrincipal(token);
            return principal != null;
        }

        public UserClaims? GetUserClaims(string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null) return null;

            return new UserClaims(
                principal.FindFirstValue(ClaimTypes.NameIdentifier)!,
                principal.FindFirstValue(ClaimTypes.Name)!,
                principal.FindFirstValue(ClaimTypes.Email),
                principal.FindFirstValue(ClaimTypes.Role)!
            );
        }

        // Validate token
        public ClaimsPrincipal? GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Value.Jwt.SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Value.Jwt.Issuer,
                    ValidAudience = _settings.Value.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
