using Backend_QLTE.ChildService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class ChildrenResponseDTO
    {
        public Guid ChildId { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? NhomTuoi { get; set; }
        public string? TrangThai { get; set; }
        public string UserId { get; set; }

    }
}
