namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class UpdateDangerZoneRequestDTO
    {
        public Guid DangerZoneId { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;

        public double ViDo { get; set; }
        public double KinhDo { get; set; }
        public double BanKinh { get; set; }
        public string? MoTa { get; set; }

    }
}
