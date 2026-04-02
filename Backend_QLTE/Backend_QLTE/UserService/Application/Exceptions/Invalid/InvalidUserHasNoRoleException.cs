using Backend_QLTE.UserService.shared.Exceptions;
using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Invalid
{
    public class InvalidUserHasNoRoleException : BusinessException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public InvalidUserHasNoRoleException(string? account)
            : base($"Tài khoản {account ?? string.Empty} không có vai trò, liên hệ quản trị viên!") { }
    }
}
