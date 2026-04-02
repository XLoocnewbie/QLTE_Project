using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserPhoneNumberException : BusinessException
    {
        public InvalidUserPhoneNumberException(string? phoneNumber)
            : base($"Số điện thoại '{phoneNumber}' không hợp lệ hoặc để trống!")
        {
        }
    }
}
