using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class SOSRequestNotFoundException : DomainException
    {
        public SOSRequestNotFoundException(Guid sosId)
            : base($"Không tìm thấy yêu cầu SOS với Id = {sosId}.") { }

        public SOSRequestNotFoundException()
            : base("Không tìm thấy yêu cầu SOS nào.") { }
    }
}
