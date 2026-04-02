using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Factories
{
    public class StudyPeriodFactory : IStudyPeriodFactory
    {
        public StudyPeriod Create(StudyPeriodCreateDTO dto)
        {
            return new StudyPeriod
            {
                StudyPeriodId = Guid.NewGuid(),
                ChildId = dto.ChildId,
                StartTime = dto.StartTime,      // ✅ DTO đã là TimeSpan → không cần TimeOfDay
                EndTime = dto.EndTime,          // ✅ giống trên
                MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim(),
                RepeatPattern = string.IsNullOrWhiteSpace(dto.RepeatPattern) ? "Daily" : dto.RepeatPattern.Trim(),
                IsActive = true,
                NgayTao = DateTime.Now
            };
        }

        public StudyPeriod Update(StudyPeriodUpdateDTO dto)
        {
            return new StudyPeriod
            {
                StudyPeriodId = dto.StudyPeriodId,
                ChildId = dto.ChildId,
                StartTime = dto.StartTime,      // ✅ không còn .TimeOfDay
                EndTime = dto.EndTime,
                MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim(),
                RepeatPattern = string.IsNullOrWhiteSpace(dto.RepeatPattern) ? "Daily" : dto.RepeatPattern.Trim(),
                IsActive = true, // vẫn giữ kích hoạt khi update
                NgayTao = DateTime.Now
            };
        }

        public StudyPeriod Delete(StudyPeriodDeleteDTO dto)
        {
            return new StudyPeriod
            {
                StudyPeriodId = dto.StudyPeriodId,
                IsActive = false // hoặc chỉ dùng để xác định Id cần xóa
            };
        }
    }
}
