using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidChildException : DomainException
    {
        public InvalidChildException(string reason)
            : base($"Thông tin trẻ không hợp lệ: {reason}.") { }
    }
}
