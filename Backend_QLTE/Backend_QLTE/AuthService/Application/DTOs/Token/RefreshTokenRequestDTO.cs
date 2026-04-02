using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token là bắt buộc!.")]
        public string RefreshToken { get; set; }
    }
}
