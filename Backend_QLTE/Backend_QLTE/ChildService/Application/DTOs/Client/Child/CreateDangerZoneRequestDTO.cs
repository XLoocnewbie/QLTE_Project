using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class CreateDangerZoneRequestDTO
    {
        [Required(ErrorMessage = "UserId bắt buộc nhập")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "ChildrenId bắt buộc nhập")]
        public string ChildrenId { get; set; }
        [Required(ErrorMessage = "TenKhuVuc bắt buộc nhập")]
        public string TenKhuVuc { get; set; }
        [Required(ErrorMessage = "Mota bắt buộc nhập")]
        public string Mota { get; set; }
        [Required(ErrorMessage = "ViDo bắt buộc nhập")]
        public double ViDo { get; set; }
        [Required(ErrorMessage = "KinhDo bắt buộc nhập")]
        public double KinhDo { get; set; }
        [Required(ErrorMessage = "BanKinh bắt buộc nhập")]
        public double BanKinh { get; set; }


    }
}
