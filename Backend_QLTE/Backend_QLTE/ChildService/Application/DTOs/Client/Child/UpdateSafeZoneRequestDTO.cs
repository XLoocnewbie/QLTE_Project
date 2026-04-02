using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class UpdateSafeZoneRequestDTO
    {
        [Required(ErrorMessage = "SafeZoneId bắt buộc nhập")]
        public Guid SafeZoneId { get; set; }
        [Required(ErrorMessage = "ChildrenId bắt buộc nhập")]
        public string TenZone { get; set; }
        [Required(ErrorMessage = "ViDo bắt buộc nhập")]
        public double ViDo { get; set; }
        [Required(ErrorMessage = "KinhDo bắt buộc nhập")]
        public double KinhDo { get; set; }
        [Required(ErrorMessage = "BanKinh bắt buộc nhập")]
        public double BanKinh { get; set; }

    }
}
