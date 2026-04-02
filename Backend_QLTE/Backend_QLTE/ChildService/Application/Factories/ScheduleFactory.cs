using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Factories
{
    public class ScheduleFactory : IScheduleFactory
    {
        // 🟢 Tạo mới Schedule
        public Schedule Create(ScheduleCreateDTO dto)
        {
            return new Schedule
            {
                ScheduleId = Guid.NewGuid(),
                ChildId = dto.ChildId,
                TenMonHoc = string.IsNullOrWhiteSpace(dto.TenMonHoc) ? "Không rõ" : dto.TenMonHoc.Trim(),
                Thu = dto.Thu,
                GioBatDau = dto.GioBatDau,
                GioKetThuc = dto.GioKetThuc
            };
        }

        // 🟡 Cập nhật Schedule
        public Schedule Update(ScheduleUpdateDTO dto)
        {
            return new Schedule
            {
                ScheduleId = dto.ScheduleId,
                ChildId = dto.ChildId,
                TenMonHoc = string.IsNullOrWhiteSpace(dto.TenMonHoc) ? "Không rõ" : dto.TenMonHoc.Trim(),
                Thu = dto.Thu,
                GioBatDau = dto.GioBatDau,
                GioKetThuc = dto.GioKetThuc
            };
        }

        // 🔴 Xóa Schedule
        public Schedule Delete(ScheduleDeleteDTO dto)
        {
            return new Schedule
            {
                ScheduleId = dto.ScheduleId
                // Có thể mở rộng thêm cờ IsDeleted nếu cần soft delete
            };
        }
    }
}
