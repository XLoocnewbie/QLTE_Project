using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Location
{
    public class CreateLocationHistoryRequestDTO
    {
        public Guid ChildId { get; set; }
        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public double? DoChinhXac { get; set; }
    }
}
