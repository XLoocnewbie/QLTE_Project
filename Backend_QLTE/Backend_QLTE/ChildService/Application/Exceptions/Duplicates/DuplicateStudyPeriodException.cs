using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateStudyPeriodException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateStudyPeriodException(TimeSpan start, TimeSpan end)
            : base($"Đã tồn tại khung giờ học trùng trong khoảng {start} - {end}.") { }
    }
}
