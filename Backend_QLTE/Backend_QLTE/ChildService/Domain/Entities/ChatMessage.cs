using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class ChatMessage
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        public string? LoaiTinNhan { get; set; } // text, image, voice,...

        public DateTime ThoiGian { get; set; } = DateTime.Now;
        public bool DaDoc { get; set; } = false;
    }

}
