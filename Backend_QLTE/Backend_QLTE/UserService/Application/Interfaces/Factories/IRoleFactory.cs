using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Interfaces.Factories
{
    public interface IRoleFactory
    {
        Role CreateRole(RoleCreateDTO roleCreate); // Tạo vai trò mới từ DTO
    }
}
