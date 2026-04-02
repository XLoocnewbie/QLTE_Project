using Backend_QLTE.ChildService.Application.DTOs.Client.Location;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class LocationSerivce : ILocationService
    {
        private readonly ChildDbContext _childDbContext;

        public LocationSerivce(ChildDbContext childDbContext)
        {
            _childDbContext = childDbContext;
        }

        public async Task<ResultDTO> CreateLocationHistoryAsync(CreateLocationHistoryRequestDTO request)
        {
            var child = await _childDbContext.Children.FirstOrDefaultAsync(c => c.ChildId == request.ChildId);
            if (child == null)
            {
                throw new ApiException($"ChildId '{request.ChildId}' không tồn tại trong hệ thống!", 404);
            }

            var create = new LocationHistory
            {
                LocationId = Guid.NewGuid(),
                ChildId = request.ChildId,
                ViDo = request.ViDo,
                KinhDo = request.KinhDo,
                DoChinhXac = request.DoChinhXac,
                ThoiGian = DateTime.Now,
            };

            var createLocation = await _childDbContext.LocationHistories.AddAsync(create);
            if(createLocation == null)
            {
                throw new ApiException($"Tạo mới location history thất bại!");
            }
            await _childDbContext.SaveChangesAsync();
            return ResultDTO.Success("Tạo mới location history thành công.");

        }

        public async Task<ResultDTO<LocationHistoryResponseDTO>> GetLocationHistoryNewAsync(Guid childId)
        {
            var child = await _childDbContext.Children.FirstOrDefaultAsync(c => c.ChildId == childId);
            if (child == null)
            {
                throw new ApiException($"ChildId '{childId}' không tồn tại trong hệ thống!", 404);
            }

            var childLocationHistory = await _childDbContext.LocationHistories
                .Where(lh => lh.ChildId == childId)
                .OrderByDescending(lh => lh.ThoiGian)
                .FirstOrDefaultAsync();
            if (childLocationHistory == null)
            {
                throw new ApiException($"ChildId '{childId}' không tồn tại trong location history!", 404);
            }
            var result = new LocationHistoryResponseDTO
            {
                LocationId = childLocationHistory.LocationId,
                ChildId = childId,
                ViDo = childLocationHistory.ViDo,
                KinhDo = childLocationHistory.KinhDo,
                DoChinhXac = childLocationHistory?.DoChinhXac,
                ThoiGian = childLocationHistory.ThoiGian
            };

            return ResultDTO<LocationHistoryResponseDTO>.Success(result, "Lấy location history mới nhất thành công");
        }
    }
}
