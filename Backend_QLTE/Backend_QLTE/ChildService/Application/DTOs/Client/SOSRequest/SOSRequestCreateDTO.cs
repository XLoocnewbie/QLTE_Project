using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest
{
    public class SOSRequestCreateDTO
    {
        [Required(ErrorMessage = "Thiếu Id của trẻ (ChildId).")]
        public Guid ChildId { get; set; }

        [Required(ErrorMessage = "Thiếu thời gian gửi yêu cầu SOS.")]
        public DateTime ThoiGian { get; set; }

        [Range(-90, 90, ErrorMessage = "Vĩ độ (ViDo) phải nằm trong khoảng -90 đến 90.")]
        public double ViDo { get; set; }

        [Range(-180, 180, ErrorMessage = "Kinh độ (KinhDo) phải nằm trong khoảng -180 đến 180.")]
        public double KinhDo { get; set; }

        [MaxLength(DtoInvariants.TrangThaiMaxLength, ErrorMessage = "Trạng thái không vượt quá {1} ký tự.")]
        public string TrangThai { get; set; } = "Đang xử lý";
    }
}
