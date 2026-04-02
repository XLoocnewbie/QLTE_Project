using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Duplicate
{
    public class DuplicateRoleNameException : DomainException
    {
        public DuplicateRoleNameException(string? roleName)
            : base($"Role name '{roleName ?? string.Empty}' đã tồn tại.")
        {
        }
    }
}
