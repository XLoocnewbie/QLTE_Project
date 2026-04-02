using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserTypeLoginException : DomainException
    {
        public InvalidUserTypeLoginException(string? userType )
            : base($"Loại tài khoản '{userType ?? string.Empty}' không hợp lệ hoặc không được để trống!")
        {
        }
    }
}
