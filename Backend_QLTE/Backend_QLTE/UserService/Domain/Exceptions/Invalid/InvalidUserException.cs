using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserException : DomainException
    {
        public InvalidUserException(string? user)
            : base($"người dùng '{user}' không hợp lệ !")
        {
        }
    }
}
