using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class StudyPeriodResponseMapper : IStudyPeriodResponseMapper
    {
        // 🟢 Entity → DTO cơ bản (GET)
        public StudyPeriodResponseDTO ToDto(StudyPeriod entity)
        {
            return new StudyPeriodResponseDTO
            {
                StudyPeriodId = entity.StudyPeriodId,
                ChildId = entity.ChildId,
                StartTime = entity.StartTime,               // ✅ giữ TimeSpan
                EndTime = entity.EndTime,
                RepeatPattern = entity.RepeatPattern ?? "Daily",
                MoTa = entity.MoTa,
                IsActive = entity.IsActive,
                NgayTao = entity.NgayTao
            };
        }

        // 🟢 Danh sách Entity → Danh sách DTO
        public List<StudyPeriodResponseDTO> ToDtoList(List<StudyPeriod> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        // 🟡 Entity → DTO sau Update
        public UpdateStudyPeriodResponseDTO ToUpdateDto(StudyPeriod entity)
        {
            return new UpdateStudyPeriodResponseDTO
            {
                StudyPeriodId = entity.StudyPeriodId,
                ChildId = entity.ChildId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                RepeatPattern = entity.RepeatPattern ?? "Daily",
                MoTa = entity.MoTa,
                IsActive = entity.IsActive
            };
        }

        // 🟡 Danh sách Entity → danh sách DTO Update
        public List<UpdateStudyPeriodResponseDTO> ToUpdateDtoList(List<StudyPeriod> entities)
        {
            return entities.Select(ToUpdateDto).ToList();
        }
    }
}
