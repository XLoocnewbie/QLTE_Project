using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidExamScheduleException : DomainException
    {
        public InvalidExamScheduleException(string reason)
            : base($"Dữ liệu lịch thi không hợp lệ: {reason}.") { }
    }
}
