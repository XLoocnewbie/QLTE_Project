using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Application.Interfaces.Mappers;
using Backend_QLTE.AuthService.Domain.Models;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.AuthService.Application.Mappers
{
    public class UserClaimsMapper : IUserClaimsMapper
    {
        public UserClaims ToDomain(UserClaimsDTO dto)
        {
            return new UserClaims(dto.UserId, dto.UserName, dto.Email, dto.Role);
        }

        public UserClaimsDTO ToDTO(UserClaims domain)
        {
            return new UserClaimsDTO
            {
                UserId = domain.UserId,
                UserName = domain.UserName,
                Email = domain.Email,
                Role = domain.Role
            };
        }
        public UserClaims FromUserServiceLoginResponse(LoginTokenResponseDTO user)
        {
            return new UserClaims(user.Data.UserId, user.Data.UserName, user.Data.Email, user.Data.Role);
               
        }
       
        public UserClaims FormUserServiceUserResponseClaims (UserResponseDTO user)
        {
            return new UserClaims(user.UserId, user.UserName, user.Email, user.Role);
        }
    }
}
