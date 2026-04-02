using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Location
{
    public class LocationHistoryResponseDTO
    {
        public Guid LocationId { get; set; }
        public Guid ChildId { get; set; }
        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public DateTime ThoiGian { get; set; }
        public double? DoChinhXac { get; set; }
    }
}
