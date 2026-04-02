using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateExamScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateExamScheduleFailedException(Guid examId)
            : base($"Cập nhật ExamSchedule Id = {examId} thất bại.") { }
    }
}
