using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class ScheduleResponseMapper : IScheduleResponseMapper
    {
        // 🟢 Entity → DTO cơ bản
        public ScheduleResponseDTO ToDto(Schedule entity)
        {
            return new ScheduleResponseDTO
            {
                ScheduleId = entity.ScheduleId,
                ChildId = entity.ChildId,
                TenMonHoc = entity.TenMonHoc,
                Thu = entity.Thu,
                GioBatDau = entity.GioBatDau,
                GioKetThuc = entity.GioKetThuc
            };
        }

        // 🟢 Danh sách Entity → DTO
        public List<ScheduleResponseDTO> ToDtoList(List<Schedule> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        // 🟡 Entity → DTO sau Update
        public UpdateScheduleResponseDTO ToUpdateDto(Schedule entity)
        {
            return new UpdateScheduleResponseDTO
            {
                ScheduleId = entity.ScheduleId,
                ChildId = entity.ChildId,
                TenMonHoc = entity.TenMonHoc,
                Thu = entity.Thu,
                GioBatDau = entity.GioBatDau,
                GioKetThuc = entity.GioKetThuc
            };
        }

        // 🟡 Danh sách Entity → DTO Update
        public List<UpdateScheduleResponseDTO> ToUpdateDtoList(List<Schedule> entities)
        {
            return entities.Select(ToUpdateDto).ToList();
        }
    }
}
