using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class DeviceInfoNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public DeviceInfoNotFoundException()
            : base("Không tìm thấy thông tin thiết bị.") { }

        public DeviceInfoNotFoundException(Guid deviceId)
            : base($"Không tìm thấy thiết bị với Id = {deviceId}.") { }
    }
}
