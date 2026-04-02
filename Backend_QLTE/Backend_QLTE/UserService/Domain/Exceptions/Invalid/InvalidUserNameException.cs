using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserNameException : BusinessException
    {
        public InvalidUserNameException(string? userName)
            : base($"Tên người dùng '{userName}' không hợp lệ hoặc để trống!")
        {
        }
    }
}
