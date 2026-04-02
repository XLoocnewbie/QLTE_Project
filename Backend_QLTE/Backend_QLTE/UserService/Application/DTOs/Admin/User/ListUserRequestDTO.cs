using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.User
{
    public class ListUserRequestDTO
    {
        [Required(ErrorMessage = "Trường 'page' là bắt buộc.")]
        public int page { get; set; } = 1;
        [Required(ErrorMessage = "Trường 'limit' là bắt buộc.")]
        public int limit { get; set; } = 10;
    }
}
