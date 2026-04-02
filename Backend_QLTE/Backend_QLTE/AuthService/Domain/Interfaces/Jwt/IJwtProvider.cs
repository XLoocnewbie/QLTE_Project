using Backend_QLTE.AuthService.Domain.Models;
using Backend_QLTE.AuthService.Domain.ValueObjects;
using System.Security.Claims;

namespace Backend_QLTE.AuthService.Domain.Interfaces.Jwt
{
    public interface IJwtProvider
    {
        Token GenerateToken(UserClaims userClaims); // Tạo token
        UserClaims? GetUserClaims(string token); // Lấy thông tin user từ token
        bool ValidateToken(string token); // Kiểm tra token có hợp lệ không
    }
}
