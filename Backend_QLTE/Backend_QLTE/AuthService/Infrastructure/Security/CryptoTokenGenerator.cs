using Backend_QLTE.AuthService.Domain.Interfaces.Services;
using System.Security.Cryptography;

namespace Backend_QLTE.AuthService.Infrastructure.Security
{
    public class CryptoTokenGenerator : ITokenGenerator
    {
        public string GenerateTokenString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
