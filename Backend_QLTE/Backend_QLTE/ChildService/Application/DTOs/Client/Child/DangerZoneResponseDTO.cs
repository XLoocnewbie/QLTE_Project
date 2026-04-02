using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class DangerZoneResponseDTO
    {
        public Guid DangerZoneId { get; set; }
        public string UserId { get; set; } = string.Empty; // ID của phụ huynh
        public string TenKhuVuc { get; set; } = string.Empty;
        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public double BanKinh { get; set; }
        public string? MoTa { get; set; }
        public Guid? ChildrenId { get; set; }
    }
}
