using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Duplicates
{
    public class DuplicateExamScheduleException : DomainException
    {
        public DuplicateExamScheduleException(string monThi, DateTime thoiGianThi)
            : base($"Môn thi '{monThi}' đã được lên lịch vào thời gian {thoiGianThi:dd/MM/yyyy HH:mm}.") { }
    }
}
