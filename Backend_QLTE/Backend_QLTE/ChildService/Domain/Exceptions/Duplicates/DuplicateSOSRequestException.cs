using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Duplicates
{
    public class DuplicateSOSRequestException : DomainException
    {
        public DuplicateSOSRequestException(Guid childId, DateTime time)
            : base($"Yêu cầu SOS cho trẻ có Id = {childId} vào lúc {time:HH:mm dd/MM/yyyy} đã tồn tại.") { }
    }
}
