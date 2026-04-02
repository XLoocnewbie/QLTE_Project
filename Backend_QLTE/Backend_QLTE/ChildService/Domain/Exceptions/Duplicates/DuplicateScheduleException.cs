using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Duplicates
{
    public class DuplicateScheduleException : DomainException
    {
        public DuplicateScheduleException(string tenMonHoc, DayOfWeek thu, TimeSpan gioBatDau, TimeSpan gioKetThuc)
            : base($"Lịch học '{tenMonHoc}' vào {thu} ({gioBatDau:hh\\:mm} - {gioKetThuc:hh\\:mm}) đã tồn tại.") { }
    }
}
