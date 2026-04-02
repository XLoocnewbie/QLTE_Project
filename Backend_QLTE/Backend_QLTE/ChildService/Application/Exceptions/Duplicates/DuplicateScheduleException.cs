using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateScheduleException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateScheduleException(string tenMonHoc, DayOfWeek thu, TimeSpan start, TimeSpan end)
            : base($"Đã tồn tại lịch học cho môn '{tenMonHoc}' vào {thu} ({start} - {end}).") { }
    }
}
