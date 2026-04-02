using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Duplicates
{
    public class DuplicateEmailException : BusinessException
    {
        public override int StatusCode => 409;
        public DuplicateEmailException(string email)
            : base($"Email '{email}' đã tồn tại. Vui lòng nhập email khác.") { }
    }
}
