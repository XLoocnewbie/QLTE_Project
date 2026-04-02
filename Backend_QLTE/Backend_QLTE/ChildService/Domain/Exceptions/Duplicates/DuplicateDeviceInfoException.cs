using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Duplicates
{
    public class DuplicateDeviceInfoException : DomainException
    {
        public DuplicateDeviceInfoException(string imei)
            : base($"Thiết bị với IMEI '{imei}' đã tồn tại trong hệ thống.") { }
    }
}
