using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidDeviceInfoException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidDeviceInfoException(string reason)
            : base($"Dữ liệu thiết bị không hợp lệ: {reason}") { }
    }
}
