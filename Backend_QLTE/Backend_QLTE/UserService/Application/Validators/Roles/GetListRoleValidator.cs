using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Exceptions.Invalid;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Validators.Roles
{
    public class GetListRoleValidator : IEntityValidator<GetListRoleRequestDTO, (List<Role> role, int total, int last)>
    {
        private readonly IRoleRepository _roleRepository;

        public GetListRoleValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task ValidateAsync(GetListRoleRequestDTO request, CancellationToken cancellationToken = default)
        {
        }

        public async Task<(List<Role> role, int total, int last)> ValidateAndGetAsync(GetListRoleRequestDTO request, CancellationToken cancellationToken = default)
        {
            var roleList = await _roleRepository.GetAllRolesAsync(request.Page, request.Limit, cancellationToken);
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
