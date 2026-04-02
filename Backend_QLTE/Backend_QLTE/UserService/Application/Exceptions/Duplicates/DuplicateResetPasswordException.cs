using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Duplicates
{
    public class DuplicateResetPasswordException : BusinessException
    {
        public override int StatusCode => 409;
        public DuplicateResetPasswordException(string? email = "")
            : base($"Mật khẩu mới trùng với mật khẩu trước: {email}") { }
    }
}
