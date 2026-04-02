using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class DeviceInfoFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public DeviceInfoFailedException(string action, string reason)
            : base($"Không thể {action} DeviceInfo: {reason}") { }
    }
}
