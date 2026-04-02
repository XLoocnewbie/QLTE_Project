using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateSOSRequestFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateSOSRequestFailedException(Guid sosId)
            : base($"Cập nhật yêu cầu SOS Id = {sosId} thất bại.") { }
    }
}
