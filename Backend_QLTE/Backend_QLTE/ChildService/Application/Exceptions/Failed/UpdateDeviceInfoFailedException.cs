using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateDeviceInfoFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateDeviceInfoFailedException(Guid deviceId)
            : base($"Cập nhật DeviceInfo Id = {deviceId} thất bại.") { }
    }
}
