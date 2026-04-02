using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.UserService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }



        // Tao Role Mới
        public async Task<IdentityResult> CreateRoleAsync(Role role)
        {
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        // Lấy tất cả Role
        public async Task<(List<Role> role , int total, int last)> GetAllRolesAsync(int page, int limit,CancellationToken cancellationToken = default)
        {
            var roles = _roleManager.Roles.AsQueryable();

            var total = await roles.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);
            var rolesList = await roles
                .OrderBy(r => r.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (rolesList,total,last);
        }

        // Lấy tất cả Role
        public async Task<Role> FindByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.FindByNameAsync(roleName);

            return roles;
        }

        // Tim Role theo id
        public async Task<Role> FindByRoleIdAsync(string roleId)
        {
            var result = await _roleManager.FindByIdAsync(roleId);
            return result;
        }

        // Lấy Role Từ RoleName
        public async Task<(List<Role> role, int total, int last)> GetByRoleNameAsync(string roleName, int page, int limit, CancellationToken cancellationToken = default)
        {
            var result = _roleManager.Roles
                .Where(r => r.Name.Contains(roleName))
                .AsQueryable();

            var total = await result.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);
            var rolesList = await result
                .OrderBy(r => r.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (rolesList,total,last);
        }

        //Update role
        public async Task<IdentityResult> UpdateRoleAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            return result;
        }

        // Xóa Role
        public async Task<IdentityResult> DeleteRoleAsync(Role role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return result;
        }
    }
}
