using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserPageException : DomainException
    {
        public InvalidUserPageException(int page)
            : base($"Số page không hợp lệ: {page}!")
        {
        }
    }
}
