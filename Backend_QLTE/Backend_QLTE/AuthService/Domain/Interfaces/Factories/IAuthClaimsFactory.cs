using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Domain.Models;
using System.Security.Claims;

namespace Backend_QLTE.AuthService.Domain.Interfaces.Factories
{
    public interface IAuthClaimsFactory
    {
        IEnumerable<Claim> CreateClaims(UserClaims userClaims); // Tạo các claim cho token
    }
}
