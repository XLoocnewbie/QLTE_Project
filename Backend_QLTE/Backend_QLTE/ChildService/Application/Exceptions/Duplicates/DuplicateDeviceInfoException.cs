using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateDeviceInfoException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateDeviceInfoException(string imei)
            : base($"Thiết bị với IMEI '{imei}' đã được đăng ký.") { }

        public DuplicateDeviceInfoException(Guid deviceId)
            : base($"Thiết bị với Id = {deviceId} đã tồn tại.") { }
    }
}
