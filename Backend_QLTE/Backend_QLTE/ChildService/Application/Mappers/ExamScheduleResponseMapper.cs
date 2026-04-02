using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class ExamScheduleResponseMapper : IExamScheduleResponseMapper
    {
        // 🟢 Entity → DTO chi tiết
        public ExamScheduleResponseDTO ToDto(ExamSchedule entity)
        {
            return new ExamScheduleResponseDTO
            {
                ExamId = entity.ExamId,
                ChildId = entity.ChildId,
                MonThi = entity.MonThi,
                ThoiGianThi = entity.ThoiGianThi,
                GhiChu = entity.GhiChu,
                DaThiXong = entity.DaThiXong,
                NgayTao = entity.NgayTao
            };
        }

        // 🟢 Danh sách Entity → Danh sách DTO
        public List<ExamScheduleResponseDTO> ToDtoList(List<ExamSchedule> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        // 🟡 Entity → DTO sau Update
        public UpdateExamScheduleResponseDTO ToUpdateDto(ExamSchedule entity)
        {
            return new UpdateExamScheduleResponseDTO
            {
                ExamId = entity.ExamId,
                ChildId = entity.ChildId,
                MonThi = entity.MonThi,
                ThoiGianThi = entity.ThoiGianThi,
                GhiChu = entity.GhiChu,
                DaThiXong = entity.DaThiXong
            };
        }

        // 🟡 Danh sách Entity → danh sách DTO Update
        public List<UpdateExamScheduleResponseDTO> ToUpdateDtoList(List<ExamSchedule> entities)
        {
            return entities.Select(ToUpdateDto).ToList();
        }
    }
}
