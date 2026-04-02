using Backend_QLTE.ChildService.Application.DTOs.Client.Location;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.Hubs
{
    public class LocationHub : Hub
    {
        private readonly ILocationService _locationService;
        private readonly IZoneService _zoneService;
        private readonly ChildDbContext _childDbContext;
        public LocationHub(ILocationService locationService, IZoneService zoneService, ChildDbContext childDbContext)
        {
            _locationService = locationService;
            _zoneService = zoneService;
            _childDbContext = childDbContext;
        }

        public async Task SendLocation(string childId, double lat, double lng) // latitude vi do,longitude kinh do
        {
            var child = await _childDbContext.Children.FirstOrDefaultAsync(c => c.ChildId == Guid.Parse(childId));
            if (child == null)
            {
                throw new ApiException(childId + " không tồn tại trong hệ thống!", 404);
            }

            // Gửi chỉ tới group của childId
            await Clients.Group(childId).SendAsync("ReceiveChildLocation", childId, lat, lng);


            var createLocation = new CreateLocationHistoryRequestDTO
            {
                ChildId = Guid.Parse(childId),
                ViDo = lat,
                KinhDo = lng,
                DoChinhXac = 0
            };
            await _locationService.CreateLocationHistoryAsync(createLocation);

            var checkSafeZone = await _zoneService.CheckSafeZoneAsync(Guid.Parse(childId), lat, lng);
            if (!checkSafeZone && child.TrangThai == "True")
            {
                // Cập nhật trạng thái trẻ ra khỏi vùng an toàn
                child.TrangThai = "False";
                _childDbContext.Children.Update(child);
                await _childDbContext.SaveChangesAsync();

                // ⚠️ Gửi thông báo đến phụ huynh trong group này
                await Clients.Group(childId).SendAsync("ChildLeftSafeZone", new
                {
                    childId,
                    tenTre = child.HoTen,
                    lat,
                    lng,
                    message = $"{child.HoTen} đã ra khỏi vùng an toàn!"
                });
            }
            else if(checkSafeZone && child.TrangThai == "False")
            {
                // Trẻ vẫn trong vùng an toàn
                child.TrangThai = "True";
                _childDbContext.Children.Update(child);
                await _childDbContext.SaveChangesAsync();
            }
        }

        // Parent join group để theo dõi childId
        public async Task JoinChildGroup(string childId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, childId);
        }

        // Parent rời group nếu cần
        public async Task LeaveChildGroup(string childId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, childId);
        }
    }
}
