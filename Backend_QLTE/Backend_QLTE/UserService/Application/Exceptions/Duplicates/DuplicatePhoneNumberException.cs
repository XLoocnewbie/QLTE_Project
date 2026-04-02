using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Duplicates
{
    public class DuplicatePhoneNumberException : BusinessException
    {
        public override int StatusCode => 409;
        public DuplicatePhoneNumberException(string phoneNumber)
            : base($"Số điện thoại '{phoneNumber}' đã tồn tại. Vui lòng nhập số điện thoại khác.") { }
    }
}
