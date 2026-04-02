using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class LocationHistory
    {
        [Key]
        public Guid LocationId { get; set; }

        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public DateTime ThoiGian { get; set; }
        public double? DoChinhXac { get; set; }
    }

}
