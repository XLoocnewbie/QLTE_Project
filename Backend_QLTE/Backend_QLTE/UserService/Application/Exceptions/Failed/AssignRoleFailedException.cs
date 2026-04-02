using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public partial class AssignRoleFailedException : BusinessException
    {
        public override int StatusCode => 500;
        public AssignRoleFailedException(string userName, string roleName)
            : base($"Gán vai trò '{roleName}' cho người dùng '{userName}' thất bại.") { }
    }
}
