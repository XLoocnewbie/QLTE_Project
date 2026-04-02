using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.AuthService.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public string RFTokenId { get; set; } // Primary Keys
        public string UserId { get; set; } // Foreign Key
        public string Token { get; set; } // Token
        public DateTime ExpiredAt { get; set; } // Thời gian hết hạn
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        public DateTime? RevokedAt { get; set; } // Thời gian thu hồi

        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiredAt;

    }
}
