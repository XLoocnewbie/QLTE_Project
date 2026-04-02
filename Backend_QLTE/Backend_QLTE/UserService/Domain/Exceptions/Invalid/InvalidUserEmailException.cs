using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserEmailException : DomainException
    {
        public InvalidUserEmailException(string? email)
            : base($"Email '{email}' không hợp lệ hoặc để trống!")
        {
        }
    }
}
