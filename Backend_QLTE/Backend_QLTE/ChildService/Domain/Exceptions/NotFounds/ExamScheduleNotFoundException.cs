using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class ExamScheduleNotFoundException : DomainException
    {
        public ExamScheduleNotFoundException(string examId)
            : base($"Không tìm thấy lịch thi với Id = {examId}.") { }
    }
}
