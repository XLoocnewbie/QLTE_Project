using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;

namespace Backend_QLTE.ChildService.Domain.Services
{
    public class ExamScheduleDomainService : IExamScheduleDomainService
    {
        public void EnsureCanCreate(ExamSchedule examSchedule, IEnumerable<ExamSchedule> existingExamSchedules)
        {
            Validate(examSchedule);

            // 🔁 Kiểm tra trùng môn thi trong cùng ngày
            bool isDuplicate = existingExamSchedules.Any(e =>
                e.MonThi.Equals(examSchedule.MonThi, StringComparison.OrdinalIgnoreCase) &&
                e.ThoiGianThi.Date == examSchedule.ThoiGianThi.Date);

            if (isDuplicate)
                throw new DuplicateExamScheduleException(examSchedule.MonThi, examSchedule.ThoiGianThi);
        }

        public void EnsureCanUpdate(ExamSchedule examSchedule)
        {
            if (examSchedule is null)
                throw new ExamScheduleNotFoundException("Không tìm thấy lịch thi để cập nhật.");

            Validate(examSchedule);
        }

        public void EnsureCanDelete(ExamSchedule examSchedule)
        {
            if (examSchedule is null)
                throw new ExamScheduleNotFoundException("Không tìm thấy lịch thi để xoá.");
        }

        private static void Validate(ExamSchedule examSchedule)
        {
            if (examSchedule is null)
                throw new InvalidExamScheduleException("Dữ liệu lịch thi không được null.");

            if (string.IsNullOrWhiteSpace(examSchedule.MonThi))
                throw new InvalidExamScheduleException("Tên môn thi không được để trống.");

            // 📅 Thời gian thi không thể là quá khứ (trừ khi đánh dấu đã thi xong)
            if (!examSchedule.DaThiXong && examSchedule.ThoiGianThi < DateTime.Now)
                throw new InvalidExamScheduleException("Thời gian thi không thể ở quá khứ.");

            // 🕒 Giờ thi phải nằm trong khoảng hợp lý (6:00 - 22:00)
            var timeOfDay = examSchedule.ThoiGianThi.TimeOfDay;
            if (timeOfDay.TotalHours < 6 || timeOfDay.TotalHours > 22)
                throw new InvalidExamScheduleException("Giờ thi chỉ được phép trong khoảng 6:00 - 22:00.");

            // 🗒️ Ghi chú không vượt quá 255 ký tự
            if (!string.IsNullOrEmpty(examSchedule.GhiChu) && examSchedule.GhiChu.Length > 255)
                throw new InvalidExamScheduleException("Ghi chú không được vượt quá 255 ký tự.");

            // 📆 Ngày tạo không thể vượt quá thời gian hiện tại nhiều hơn 5 phút
            if (examSchedule.NgayTao > DateTime.Now.AddMinutes(5))
                throw new InvalidExamScheduleException("Ngày tạo không thể ở tương lai.");
        }
    }
}
