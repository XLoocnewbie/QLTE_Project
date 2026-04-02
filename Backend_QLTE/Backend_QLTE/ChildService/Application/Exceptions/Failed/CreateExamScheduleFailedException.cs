using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateExamScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateExamScheduleFailedException(string childId)
            : base($"Tạo ExamSchedule cho ChildId = {childId} thất bại.") { }
    }
}
