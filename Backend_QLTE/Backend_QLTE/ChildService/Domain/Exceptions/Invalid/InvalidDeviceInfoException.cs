using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidDeviceInfoException : DomainException
    {
        public InvalidDeviceInfoException(string reason)
            : base($"Thông tin thiết bị không hợp lệ: {reason}.") { }
    }
}
