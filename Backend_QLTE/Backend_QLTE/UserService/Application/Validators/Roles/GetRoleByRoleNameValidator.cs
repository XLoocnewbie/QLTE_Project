using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Exceptions.Invalid;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Validators.Roles
{
    public class GetRoleByRoleNameValidator : IEntityValidator<FindRoleByRoleNameRequestDTO, (List<Role> role, int total, int last)>
    {
        private readonly IRoleRepository _roleRepository;
        public GetRoleByRoleNameValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task ValidateAsync(FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default)
        {

        }

        public async Task<(List<Role> role, int total, int last)> ValidateAndGetAsync(FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default)
        {
            var roleList = await _roleRepository.GetByRoleNameAsync(request.RoleName, request.Page, request.Limit);
            if (roleList.total == 0)
            {
                throw new RoleNotFoundException();
            }

            if (roleList.last < request.Page)
            {
                throw new InvalidPageIndexOutOfRangeException(request.Page, roleList.total);
            }

            return roleList;
        }
    }
}
