using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidStudyPeriodException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidStudyPeriodException(string reason)
            : base($"Dữ liệu StudyPeriod không hợp lệ: {reason}") { }
    }
}
