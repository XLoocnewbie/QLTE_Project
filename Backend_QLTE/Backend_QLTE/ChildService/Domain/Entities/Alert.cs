using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class Alert
    {
        [Key]
        public Guid AlertId { get; set; }

        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        [Required, MaxLength(50)]
        public string LoaiCanhBao { get; set; } = string.Empty; // OutOfSafeZone, SOS,...

        public string? NoiDung { get; set; }

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        public bool DaXuLy { get; set; } = false;
    }

}
