using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class LogOutRequestDTO
    {
        [Required(ErrorMessage ="Refresh Token là bắt buộc!")]
        public string RefreshToken { get; set; }
    }
}
