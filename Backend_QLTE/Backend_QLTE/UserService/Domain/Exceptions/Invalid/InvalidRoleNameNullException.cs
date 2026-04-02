using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidRoleNameNullException : DomainException
    {
        public InvalidRoleNameNullException(string? roleName)
            : base($"Tên vai trò '{roleName ?? string.Empty}' không được để trống!")
        {
        }
    }
}
