using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Factories
{
    public class SOSRequestFactory : ISOSRequestFactory
    {
        public SOSRequest Create(SOSRequestCreateDTO dto)
        {
            return new SOSRequest
            {
                SOSId = Guid.NewGuid(),
                ChildId = dto.ChildId,
                ThoiGian = dto.ThoiGian,
                ViDo = dto.ViDo,
                KinhDo = dto.KinhDo,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "Đang xử lý" : dto.TrangThai.Trim()
            };
        }

        public SOSRequest Update(SOSRequestUpdateDTO dto)
        {
            return new SOSRequest
            {
                SOSId = dto.SOSId,
                ChildId = dto.ChildId,
                ThoiGian = dto.ThoiGian,
                ViDo = dto.ViDo,
                KinhDo = dto.KinhDo,
                TrangThai = string.IsNullOrWhiteSpace(dto.TrangThai) ? "Đang xử lý" : dto.TrangThai.Trim()
            };
        }

        public SOSRequest Delete(SOSRequestDeleteDTO dto)
        {
            return new SOSRequest
            {
                SOSId = dto.SOSId
            };
        }
    }
}
