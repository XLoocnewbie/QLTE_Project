using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class SOSRequestResponseMapper : ISOSRequestResponseMapper
    {
        // 🟢 Entity → DTO cơ bản (GET)
        public SOSRequestResponseDTO ToDto(SOSRequest entity)
        {
            return new SOSRequestResponseDTO
            {
                SOSId = entity.SOSId,
                ChildId = entity.ChildId,
                ThoiGian = entity.ThoiGian,
                ViDo = entity.ViDo,
                KinhDo = entity.KinhDo,
                TrangThai = entity.TrangThai ?? "Đang xử lý"
            };
        }

        // 🟢 Danh sách Entity → Danh sách DTO
        public List<SOSRequestResponseDTO> ToDtoList(List<SOSRequest> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        // 🟡 Entity → DTO sau Update
        public UpdateSOSRequestResponseDTO ToUpdateDto(SOSRequest entity)
        {
            return new UpdateSOSRequestResponseDTO
            {
                SOSId = entity.SOSId,
                ChildId = entity.ChildId,
                ThoiGian = entity.ThoiGian,
                ViDo = entity.ViDo,
                KinhDo = entity.KinhDo,
                TrangThai = entity.TrangThai ?? "Đang xử lý"
            };
        }

        // 🟡 Danh sách Entity → danh sách DTO Update
        public List<UpdateSOSRequestResponseDTO> ToUpdateDtoList(List<SOSRequest> entities)
        {
            return entities.Select(ToUpdateDto).ToList();
        }
    }
}
