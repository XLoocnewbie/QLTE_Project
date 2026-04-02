using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class DeviceInfo
    {
        [Key]
        public Guid DeviceId { get; set; }

        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        [MaxLength(100)]
        public string? TenThietBi { get; set; }

        [MaxLength(50)]
        public string? IMEI { get; set; }

        public int? Pin { get; set; }

        public bool TrangThaiOnline { get; set; }

        public DateTime LanCapNhatCuoi { get; set; }
        public bool IsTracking { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public ICollection<DeviceRestriction> DeviceRestrictions { get; set; } = new List<DeviceRestriction>();
    }

}
