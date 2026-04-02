using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidChangeEmailException : DomainException
    {
        public InvalidChangeEmailException(string email) : base($"Bạn chỉ có thể thay đổi email '{email}' sau 30 ngày thay đổi!")
        {
        }
    }
}
