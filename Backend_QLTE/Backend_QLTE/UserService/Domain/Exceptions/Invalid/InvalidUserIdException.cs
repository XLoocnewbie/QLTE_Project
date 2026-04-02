using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserIdException : DomainException
    {
        public InvalidUserIdException(string? userId)
            : base($"UserId '{userId}' không hợp lệ hoặc để trống!")
        {
        }
    }
}
