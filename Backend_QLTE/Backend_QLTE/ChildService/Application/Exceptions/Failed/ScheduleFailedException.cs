using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class ScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public ScheduleFailedException(string action, string reason)
            : base($"Không thể {action} Schedule: {reason}.") { }
    }
}
