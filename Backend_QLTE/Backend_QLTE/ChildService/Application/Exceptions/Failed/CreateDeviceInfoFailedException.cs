using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateDeviceInfoFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateDeviceInfoFailedException(Guid childId)
            : base($"Tạo DeviceInfo cho ChildId = {childId} thất bại.") { }
    }
}
