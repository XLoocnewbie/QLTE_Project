using System;
using System.Collections.Generic;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class ChildSummaryDTO
    {
        public Guid ChildId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public string AnhDaiDien { get; set; } = string.Empty;
        public string NhomTuoi { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
