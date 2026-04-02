using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Duplicates
{
    public class DuplicateRoleNameException : BusinessException
    {
        public override int StatusCode => 409;
        public DuplicateRoleNameException(string roleName)
            : base($"Tên vai trò '{roleName}' đã tồn tại.") { }
    }
}
