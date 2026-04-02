using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Interfaces.Factories;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Factories
{
    public class RoleFactory : IRoleFactory
    {
        // Tạo vai trò mới từ DTO
        public Role CreateRole(RoleCreateDTO roleCreate)
        {
            return new Role
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = roleCreate.RoleName,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            };
        }
    }
}
