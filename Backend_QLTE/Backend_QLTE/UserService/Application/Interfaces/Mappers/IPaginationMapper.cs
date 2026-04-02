
using Backend_QLTE.UserService.Application.DTOs.Common;

namespace Backend_QLTE.UserService.Application.Interfaces.Mappers
{
    public interface IPaginationMapper
    {
        public PaginationDTO ToDto(int page, int limit, int total, int last);
    }
}
