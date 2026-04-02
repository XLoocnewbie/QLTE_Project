using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class ExamScheduleFailedException : DomainException
    {
        public ExamScheduleFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với lịch thi thất bại: {reason}.") { }
    }
}
