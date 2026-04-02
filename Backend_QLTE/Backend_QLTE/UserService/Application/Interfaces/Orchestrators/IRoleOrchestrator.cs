using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Interfaces.Orchestrators
{
    public interface IRoleOrchestrator
    {
        Task<Role> CreateRoleAsync(RoleCreateDTO createRole, CancellationToken cancellationToken = default); // Tạo Role mới
        Task<(List<Role> role, int total, int last)> GetAllRoleAsync(GetListRoleRequestDTO request, CancellationToken cancellationToken = default); // Lấy tất cả Role
        Task<(List<Role> role, int total, int last)> GetRoleByRoleNameAsync(FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default); // Tìm Role theo tên
    }
}
