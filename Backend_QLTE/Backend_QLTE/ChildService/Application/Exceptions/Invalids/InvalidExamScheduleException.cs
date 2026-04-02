using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidExamScheduleException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidExamScheduleException(string reason)
            : base($"Dữ liệu ExamSchedule không hợp lệ: {reason}.") { }
    }
}
