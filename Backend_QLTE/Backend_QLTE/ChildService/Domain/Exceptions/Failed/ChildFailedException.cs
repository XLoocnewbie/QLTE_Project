using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class ChildFailedException : DomainException
    {
        public ChildFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với thông tin trẻ thất bại: {reason}.") { }
    }
}
