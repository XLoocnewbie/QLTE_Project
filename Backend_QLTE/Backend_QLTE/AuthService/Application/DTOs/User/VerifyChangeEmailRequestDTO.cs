using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Application.DTOs.User
{
    public class VerifyChangeEmailRequestDTO
    {
        [Required(ErrorMessage = "NewEmail bắt buộc phải nhập!")]
        [EmailAddress(ErrorMessage = "NewEmail không đúng định dạng!")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Otp là bắt buộc!")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Type Otp là bắt buộc!")]
        public string Type { get; set; } = "ChangeEmail";

    }
}
