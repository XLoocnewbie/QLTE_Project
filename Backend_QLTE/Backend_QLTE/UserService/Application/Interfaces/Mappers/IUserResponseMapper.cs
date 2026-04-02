using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Interfaces.Mappers
{
    public interface IUserResponseMapper
    {
        UpdateInfoUserResponseDTO ToUpdate(User user); // Map Cập nhật thông tin user
        ExternalLoginUserResponseDTO ToExternalLogin(User user); // Map Đăng nhập bên ngoài
        LoginLocalUserResponseDTO ToLocalLogin(User user); // Map Đăng nhập local
        UserResponseDTO ToDto(User user); // Map User sang UserResponseDTO
    }
}
