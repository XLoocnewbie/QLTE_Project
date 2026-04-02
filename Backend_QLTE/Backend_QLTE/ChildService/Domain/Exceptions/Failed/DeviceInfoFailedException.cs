using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class DeviceInfoFailedException : DomainException
    {
        public DeviceInfoFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với thông tin thiết bị thất bại: {reason}.") { }
    }
}
