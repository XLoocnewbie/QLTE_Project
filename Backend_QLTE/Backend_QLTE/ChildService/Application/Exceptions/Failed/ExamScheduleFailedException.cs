using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class ExamScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public ExamScheduleFailedException(string action, string reason)
            : base($"Không thể {action} ExamSchedule: {reason}.") { }
    }
}
