using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Factories
{
    public class ExamScheduleFactory : IExamScheduleFactory
    {
        public ExamSchedule Create(ExamScheduleCreateDTO dto)
        {
            return new ExamSchedule
            {
                ExamId = Guid.NewGuid(),
                ChildId = dto.ChildId,
                MonThi = dto.MonThi.Trim(),
                ThoiGianThi = dto.ThoiGianThi,
                GhiChu = string.IsNullOrWhiteSpace(dto.GhiChu) ? null : dto.GhiChu.Trim(),
                DaThiXong = false,
                NgayTao = DateTime.Now
            };
        }

        public ExamSchedule Update(ExamScheduleUpdateDTO dto)
        {
            return new ExamSchedule
            {
                ExamId = dto.ExamId,
                ChildId = dto.ChildId,
                MonThi = dto.MonThi.Trim(),
                ThoiGianThi = dto.ThoiGianThi,
                GhiChu = string.IsNullOrWhiteSpace(dto.GhiChu) ? null : dto.GhiChu.Trim(),
                DaThiXong = dto.DaThiXong,
                NgayTao = DateTime.Now
            };
        }

        public ExamSchedule Delete(ExamScheduleDeleteDTO dto)
        {
            return new ExamSchedule
            {
                ExamId = dto.ExamId
            };
        }
    }
}
