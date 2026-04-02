using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class PaginationMapper : IPaginationMapper
    {
        public PaginationDTO ToDto(int page, int limit, int total,int last)
        {
            return new PaginationDTO
            {
                Page = page,
                Limit = limit,
                Last = last,
                Total = total
            };
        }
    }
}
