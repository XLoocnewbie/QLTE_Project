using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Duplicate
{
    public class DuplicateEmailException : DomainException
    {
        public DuplicateEmailException(string email) : base($"Email '{email}' đã tồn tại trong hệ thống!")
        {
        }
    }
}
