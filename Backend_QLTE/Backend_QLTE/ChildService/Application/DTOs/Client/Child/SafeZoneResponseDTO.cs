using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class SafeZoneResponseDTO
    {
        public Guid SafeZoneId { get; set; }
        public string UserId { get; set; } // ID của phụ huynh

        public string TenZone { get; set; } 

        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public double BanKinh { get; set; }
        public Guid? ChildrenId { get; set; }
    }
}
