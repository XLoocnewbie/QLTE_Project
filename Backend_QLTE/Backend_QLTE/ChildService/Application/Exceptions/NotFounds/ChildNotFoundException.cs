using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class ChildNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public ChildNotFoundException()
            : base("Không tìm thấy thông tin trẻ.") { }

        public ChildNotFoundException(Guid childId)
            : base($"Không tìm thấy trẻ với Id = {childId}.") { }

        public ChildNotFoundException(string message)
            : base(message) { }
    }
}
