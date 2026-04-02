using Backend_QLTE.UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Backend_QLTE.UserService.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<IdentityResult> CreateRoleAsync(Role role); // Tạo Role mới
        Task<(List<Role> role, int total, int last)> GetByRoleNameAsync(string roleName, int page, int limit, CancellationToken cancellationToken = default); // Lấy Role Từ RoleName
        Task<(List<Role> role, int total, int last)> GetAllRolesAsync(int page, int limit, CancellationToken cancellationToken = default); // Lấy tất cả Role
        Task<Role> FindByRoleIdAsync(string roleId); // Lấy Role Từ RoleId
        Task<Role> FindByRoleNameAsync(string roleName, CancellationToken cancellationToken = default); // Lấy Role Từ RoleName
        Task<IdentityResult> UpdateRoleAsync(Role role); // Cập nhật Role
        Task<IdentityResult> DeleteRoleAsync(Role role); // Xóa Role
    }
}
