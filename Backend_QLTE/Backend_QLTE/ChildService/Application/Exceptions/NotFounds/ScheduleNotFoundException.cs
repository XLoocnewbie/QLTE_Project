using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class ScheduleNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public ScheduleNotFoundException()
            : base("Không tìm thấy bất kỳ Schedule nào.") { }

        public ScheduleNotFoundException(Guid scheduleId)
            : base($"Không tìm thấy Schedule với Id = {scheduleId}.") { }
    }
}
