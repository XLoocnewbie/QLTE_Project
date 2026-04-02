using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Message
{
    public class MessageResponseDTO
    {
        public Guid MessageId { get; set; }
        public string SenderId { get; set; } 
        public string ReceiverId { get; set; } 
        public string NoiDung { get; set; } 
        public string? LoaiTinNhan { get; set; } 
        public DateTime ThoiGian { get; set; } 
        public bool DaDoc { get; set; } 
    }
}
