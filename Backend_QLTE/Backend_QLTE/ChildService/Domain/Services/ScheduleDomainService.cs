using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;

namespace Backend_QLTE.ChildService.Domain.Services
{
    public class ScheduleDomainService : IScheduleDomainService
    {
        public void EnsureCanCreate(Schedule schedule, IEnumerable<Schedule> existingSchedules)
        {
            Validate(schedule);

            // 🔁 Kiểm tra trùng trong cùng thứ & giờ học
            bool isOverlap = existingSchedules.Any(s =>
                s.Thu == schedule.Thu &&
                ((schedule.GioBatDau < s.GioKetThuc) && (schedule.GioKetThuc > s.GioBatDau)));

            if (isOverlap)
                throw new DuplicateScheduleException(schedule.TenMonHoc, schedule.Thu, schedule.GioBatDau, schedule.GioKetThuc);
        }

        public void EnsureCanUpdate(Schedule schedule)
        {
            if (schedule is null)
                throw new ScheduleNotFoundException(Guid.Empty);

            Validate(schedule);
        }

        public void EnsureCanDelete(Schedule schedule)
        {
            if (schedule is null)
                throw new ScheduleNotFoundException(Guid.Empty);
        }

        private static void Validate(Schedule schedule)
        {
            if (schedule is null)
                throw new InvalidScheduleException("Dữ liệu lịch học không được null.");

            if (string.IsNullOrWhiteSpace(schedule.TenMonHoc))
                throw new InvalidScheduleException("Thiếu tên môn học.");

            if (schedule.GioBatDau >= schedule.GioKetThuc)
                throw new InvalidScheduleException("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");

            var duration = (schedule.GioKetThuc - schedule.GioBatDau).TotalMinutes;
            if (duration < 15)
                throw new InvalidScheduleException("Lịch học quá ngắn (<15 phút).");

            if (!Enum.IsDefined(typeof(DayOfWeek), schedule.Thu))
                throw new InvalidScheduleException("Thứ trong tuần không hợp lệ.");

            if (schedule.GioBatDau.TotalHours < 6 || schedule.GioKetThuc.TotalHours > 22)
                throw new InvalidScheduleException("Giờ học chỉ được phép trong khoảng 6:00 - 22:00.");
        }
    }
}
