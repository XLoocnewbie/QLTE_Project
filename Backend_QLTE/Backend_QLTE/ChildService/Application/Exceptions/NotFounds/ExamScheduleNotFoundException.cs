using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class ExamScheduleNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public ExamScheduleNotFoundException()
            : base("Không tìm thấy bất kỳ ExamSchedule nào.") { }

        public ExamScheduleNotFoundException(Guid examId)
            : base($"Không tìm thấy ExamSchedule với Id = {examId}.") { }
    }
}
