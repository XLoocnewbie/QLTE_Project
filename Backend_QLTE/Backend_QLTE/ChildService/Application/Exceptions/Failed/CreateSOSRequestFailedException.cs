using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateSOSRequestFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateSOSRequestFailedException(Guid childId)
            : base($"Tạo yêu cầu SOS cho ChildId = {childId} thất bại.") { }
    }
}
