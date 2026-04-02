using System;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest
{
    public class SOSRequestResponseDTO
    {
        public Guid SOSId { get; set; }
        public Guid ChildId { get; set; }
        public DateTime ThoiGian { get; set; }
        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }
}
