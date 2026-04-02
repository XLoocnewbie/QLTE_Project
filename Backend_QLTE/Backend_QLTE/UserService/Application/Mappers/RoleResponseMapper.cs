using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Mappers
{
    public class RoleResponseMapper  : IRoleResponseMapper
    {
        // Chuyển đổi từ Role entity sang RoleResponseDTO
        public RoleResponseDTO toDto (Role role)
        {
            return new RoleResponseDTO
            {
                RoleId = role.Id,
                RoleName = role.Name,
                ThoiGianTao = role.ThoiGianTao,
                ThoiGianCapNhat = role.ThoiGianCapNhat
            };
        }
    }
}
