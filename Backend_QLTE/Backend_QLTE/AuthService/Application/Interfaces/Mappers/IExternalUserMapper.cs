using Backend_QLTE.AuthService.Application.DTOs.Login;

namespace Backend_QLTE.AuthService.Application.Interfaces.Mappers
{
    public interface IExternalUserMapper 
    {
        ExternalAuthUserInfoDTO ToAuthDTO(ExternalLoginUserInfoDTO external); // chuyển từ DTO bên cung cấp về DTO bên mình
    }
}
