using Backend_QLTE.AuthService.Domain.Entities;
using Backend_QLTE.AuthService.Domain.ValueObjects;

namespace Backend_QLTE.AuthService.Application.Interfaces.Factories
{
    public interface ITokenFactory
    {
        RefreshToken CreateRefreshToken(RefreshTokenString refreshToken, string userId); // Tạo đối tượng RefreshToken từ chuỗi refresh token và userId
    }
}
