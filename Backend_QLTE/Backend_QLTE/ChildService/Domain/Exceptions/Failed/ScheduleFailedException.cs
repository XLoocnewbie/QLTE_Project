using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class ScheduleFailedException : DomainException
    {
        public ScheduleFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với lịch học thất bại: {reason}.") { }
    }
}
