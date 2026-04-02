using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Exceptions.Duplicates;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;

namespace Backend_QLTE.UserService.Application.Validators.Roles
{
    public class CreateRoleNameValidator : IValidator<RoleCreateDTO>
    {
        private readonly IRoleRepository _roleRepository;
        public CreateRoleNameValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task ValidateAsync(RoleCreateDTO roleCreate, CancellationToken cancellationToken = default)
        {
            var existingRole = await _roleRepository.FindByRoleNameAsync(roleCreate.RoleName);
            if (existingRole != null)
                throw new DuplicateRoleNameException(roleCreate.RoleName);
        }
    }
}
