using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Interfaces.Factories;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Factories
{
    public class UserFactory : IUserFactory
    {
        // Tạo user local từ thông tin đăng ký
        public User CreateLocalUser(UserRegisterDTO userRegister, string userName)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = userRegister.Email,
                UserName = userName,
                PhoneNumber = userRegister.PhoneNumber,
                TypeLogin = "Local",
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow,
                ThoiGianDoiEmail = DateTime.UtcNow
            };
        }

        public User CreateExternalUser(ExternalLoginUserInfoDTO externalUser, string userName)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = externalUser.Email,
                NameND = externalUser.NameND,
                UserName = userName,
                AvatarND = externalUser.AvatarND,
                TypeLogin = externalUser.TypeLogin,
                AuthId = externalUser.AuthId,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow,
                ThoiGianDoiEmail = DateTime.UtcNow
            };
        }

        public User CreateUpdateInfoUser(InfoUserUpdateRequestDTO update, string userId)
        {
            return new User
            {
                Id = userId,
                UserName = update.UserName,
                PhoneNumber = update.PhoneNumber,
                NameND = update.NameND,
                AvatarND = update.AvatarND?.ToString(),
                ThoiGianCapNhat = DateTime.UtcNow,
                
            };
        }

        public User CreateEmail(FindUserByEmailRequestDTO dto)
        {
            return new User
            {
                Email = dto.Email
            };
        }

        public User CreateUserId(FindUserByIdRequestDTO dto)
        {
            return new User
            {
                Id = dto.UserId
            };
        }

        public User CreateResetPassword(ResetPasswordRequestDTO dto)
        {
            return new User
            {
                Id = dto.UserId,
                ThoiGianCapNhat = DateTime.UtcNow
            };
        }

    }
}
