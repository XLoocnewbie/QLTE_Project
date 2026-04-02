using System.Collections.Generic;

namespace Backend_QLTE.AuthService.Application.DTOs.Token
{
    public class RefreshTokenListDTO
    {
        public List<RefreshTokenInfoDTO> Items { get; set; } = new();
    }
}
