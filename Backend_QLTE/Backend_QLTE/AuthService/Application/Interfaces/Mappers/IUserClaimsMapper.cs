using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Domain.Models;

namespace Backend_QLTE.AuthService.Application.Interfaces.Mappers
{
    public interface IUserClaimsMapper
    {
        UserClaims ToDomain(UserClaimsDTO dto); // Chuyển từ DTO sang Domain
        UserClaimsDTO ToDTO(UserClaims domain); // Chuyển từ Domain sang DTO
        UserClaims FromUserServiceLoginResponse(LoginTokenResponseDTO user); // Chuyển từ phản hồi đăng nhập của UserService sang UserClaimsDTO
        UserClaims FormUserServiceUserResponseClaims(UserResponseDTO user); // Chuyển từ phản hồi thông tin người dùng của UserService sang UserClaimsDTO
    }
}
