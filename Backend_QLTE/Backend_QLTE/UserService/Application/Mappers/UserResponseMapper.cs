using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Mappers
{
    public class UserResponseMapper : IUserResponseMapper
    {
        // Map Cập nhật thông tin
        //
        // user
        public UpdateInfoUserResponseDTO ToUpdate(User user)
        {
            return new UpdateInfoUserResponseDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NameND = user.NameND,
                GioiTinh = user.GioiTinh ?? 0,
                Mota = user.MoTa,
                AvatarND = user.AvatarND,
                ThoiGianCapNhat = user.ThoiGianCapNhat ?? DateTime.Now,
                ThoiGianTao = user.ThoiGianTao
            };
        }

        // Map Đăng nhập bên ngoài
        public ExternalLoginUserResponseDTO ToExternalLogin(User user)
        {
            return new ExternalLoginUserResponseDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Roles.FirstOrDefault(),
            };
        }

        // Map Đăng nhập local
        public LoginLocalUserResponseDTO ToLocalLogin(User user)
        {
            return new LoginLocalUserResponseDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Roles.FirstOrDefault(),
            };
        }

        // Map User sang UserResponseDTO
        public UserResponseDTO ToDto(User user)
        {
            return new UserResponseDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NameND = user.NameND,
                GioiTinh = user.GioiTinh ?? 0,
                Mota = user.MoTa,
                AvatarND = user.AvatarND,
                TypeLogin = user.TypeLogin,
                Role = user.Roles.FirstOrDefault(),
                ThoiGianCapNhat = user.ThoiGianCapNhat ?? DateTime.Now,
                ThoiGianTao = user.ThoiGianTao
            };
        }
    }
}
