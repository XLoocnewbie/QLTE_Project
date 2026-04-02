using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface IPaginationMapper
    {
        public PaginationDTO ToDto(int page, int limit, int total, int last);
    }
}
