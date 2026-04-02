using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;

namespace Backend_QLTE.ChildService.Domain.Services
{
    public class StudyPeriodDomainService : IStudyPeriodDomainService
    {
        public void EnsureCanCreate(StudyPeriod studyPeriod)
        {
            Validate(studyPeriod);
        }

        public void EnsureCanUpdate(StudyPeriod studyPeriod)
        {
            if (studyPeriod is null)
                throw new StudyPeriodNotFoundException("Không tìm thấy khung giờ học để cập nhật.");

            Validate(studyPeriod);
        }

        public void EnsureCanDelete(StudyPeriod studyPeriod)
        {
            if (studyPeriod is null)
                throw new StudyPeriodNotFoundException("Không tìm thấy khung giờ học để xoá.");
        }

        private static void Validate(StudyPeriod studyPeriod)
        {
            if (studyPeriod == null)
                throw new InvalidStudyPeriodException("Dữ liệu khung giờ học không được null.");

            if (studyPeriod.StartTime >= studyPeriod.EndTime)
                throw new InvalidStudyPeriodException("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");

            var duration = (studyPeriod.EndTime - studyPeriod.StartTime).TotalMinutes;
            if (duration < 15)
                throw new InvalidStudyPeriodException("Khung giờ học quá ngắn (<15 phút).");

            if (string.IsNullOrWhiteSpace(studyPeriod.RepeatPattern))
                throw new InvalidStudyPeriodException("Thiếu thông tin về kiểu lặp lại (RepeatPattern).");

            if (studyPeriod.NgayTao > DateTime.Now.AddMinutes(5))
                throw new InvalidStudyPeriodException("Ngày tạo không thể ở tương lai.");
        }
    }
}
