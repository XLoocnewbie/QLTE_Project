using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class SOSRequest
    {
        [Key]
        public Guid SOSId { get; set; }

        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        public DateTime ThoiGian { get; set; }

        public double ViDo { get; set; }
        public double KinhDo { get; set; }

        public string TrangThai { get; set; } = "Đang xử lý";
    }

}
