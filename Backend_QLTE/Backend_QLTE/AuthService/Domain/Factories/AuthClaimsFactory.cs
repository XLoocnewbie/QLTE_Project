using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Domain.Interfaces.Factories;
using Backend_QLTE.AuthService.Domain.Models;
using System.Security.Claims;

namespace Backend_QLTE.AuthService.Domain.Factories
{
    // Tạo các claim cho token
    public class AuthClaimsFactory : IAuthClaimsFactory
    {
        public IEnumerable<Claim> CreateClaims(UserClaims userClaims)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userClaims.UserId),
                new Claim(ClaimTypes.Name, userClaims.UserName),
                new Claim(ClaimTypes.Email, userClaims.Email ?? ""),
                new Claim(ClaimTypes.Role, userClaims.Role)
            };
        }
    }
}
