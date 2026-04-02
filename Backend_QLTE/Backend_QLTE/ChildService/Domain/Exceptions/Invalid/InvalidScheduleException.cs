using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidScheduleException : DomainException
    {
        public InvalidScheduleException(string reason)
            : base($"Lịch học không hợp lệ: {reason}.") { }
    }
}
