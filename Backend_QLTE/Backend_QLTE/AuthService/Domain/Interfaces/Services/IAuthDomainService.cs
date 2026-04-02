using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Domain.Entities;
using Backend_QLTE.AuthService.Domain.Models;
using Backend_QLTE.AuthService.Domain.ValueObjects;

namespace Backend_QLTE.AuthService.Domain.Interfaces.Services
{
    public interface IAuthDomainService
    {
        Token GenerateToken(UserClaims userClaims); // Tạo Token Cho client
        RefreshTokenString GenarateRefreshToken(); // Tạo mã refresh token
        bool ValidateToken(string token); // Kiểm tra xem token có hợp lệ không
        void IsActiveRefreshToken(RefreshToken refreshToken); // Kiểm tra xem mã refresh token còn hoạt động không
        void RevokeRefreshToken(RefreshToken refreshToken); // Thu hồi mã refresh token
        UserClaims GetUserFromToken(string token); // Lấy thông tin người dùng từ token
    }
}
