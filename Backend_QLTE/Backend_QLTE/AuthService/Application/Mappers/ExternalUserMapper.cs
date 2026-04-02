using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.Interfaces.Mappers;

namespace Backend_QLTE.AuthService.Application.Mappers
{
    public class ExternalUserMapper : IExternalUserMapper
    {
        public ExternalAuthUserInfoDTO ToAuthDTO(ExternalLoginUserInfoDTO external)
        {
           return new ExternalAuthUserInfoDTO
           {
               Email = external.Email,
               AuthId = external.AuthId,
               NameND = external.NameND,
               AvatarND = external.AvatarND,
               TypeLogin = external.Provider
           };
        }
    }
}
