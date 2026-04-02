using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class ScheduleNotFoundException : DomainException
    {
        public ScheduleNotFoundException(Guid scheduleId)
            : base($"Không tìm thấy lịch học với Id = {scheduleId}.") { }

        public ScheduleNotFoundException(string message)
            : base(message) { }
    }
}
