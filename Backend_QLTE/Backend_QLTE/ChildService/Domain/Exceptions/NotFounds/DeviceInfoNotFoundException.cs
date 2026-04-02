using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class DeviceInfoNotFoundException : DomainException
    {
        public DeviceInfoNotFoundException(string deviceId)
            : base($"Không tìm thấy thiết bị với Id = {deviceId}.") { }
    }
}
