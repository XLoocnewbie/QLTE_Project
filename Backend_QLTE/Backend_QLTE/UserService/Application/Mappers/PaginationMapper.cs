
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Application.DTOs.Common;

namespace Backend_QLTE.UserService.Application.Mappers
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
