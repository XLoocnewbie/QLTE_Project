using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class SOSRequestNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public SOSRequestNotFoundException()
            : base("Không tìm thấy bất kỳ yêu cầu SOS nào.") { }

        public SOSRequestNotFoundException(Guid sosId)
            : base($"Không tìm thấy yêu cầu SOS với Id = {sosId}.") { }
    }
}
