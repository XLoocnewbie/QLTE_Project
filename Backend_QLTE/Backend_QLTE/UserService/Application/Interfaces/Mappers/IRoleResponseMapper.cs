using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Interfaces.Mappers
{
    public interface IRoleResponseMapper
    {
        RoleResponseDTO toDto(Role role); // Chuyển đổi Role thành RoleResponseDTO
    }
}
