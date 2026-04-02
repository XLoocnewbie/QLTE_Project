using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class ChildNotFoundException : DomainException
    {
        public ChildNotFoundException(string childId)
            : base($"Không tìm thấy thông tin của trẻ với Id = {childId}.") { }

        public ChildNotFoundException(Guid childId)
            : base($"Không tìm thấy thông tin của trẻ với Id = {childId}.") { }

        public ChildNotFoundException()
            : base("Không tìm thấy thông tin của trẻ.") { }
    }
}
