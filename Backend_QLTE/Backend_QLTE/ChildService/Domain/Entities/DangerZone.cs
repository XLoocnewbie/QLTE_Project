using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class DangerZone
    {
        [Key]
        public Guid DangerZoneId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // ID của phụ huynh

        [Required, MaxLength(100)]
        public string TenKhuVuc { get; set; } = string.Empty;

        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public double BanKinh { get; set; }
        public string? MoTa { get; set; }
        [ForeignKey(nameof(Child))]
        public Guid? ChildrenId { get; set; }
        public Child Child {  get; set; }   
    }

}
