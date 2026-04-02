using Microsoft.AspNetCore.SignalR;

namespace Backend_QLTE.ChildService.Api.Hubs
{
    public class DeviceHub : Hub
    {
        private readonly ILogger<DeviceHub> _logger;

        public DeviceHub(ILogger<DeviceHub> logger)
        {
            _logger = logger;
        }

        // 🔹 Khi client (phụ huynh hoặc thiết bị) kết nối
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client {ConnectionId} đã kết nối tới DeviceHub.", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // 🔹 Khi client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client {ConnectionId} đã ngắt kết nối khỏi DeviceHub.", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // 🔹 Tham gia vào nhóm theo ChildId (mỗi đứa trẻ có group riêng)
        public async Task JoinChildGroup(Guid childId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, childId.ToString());
            _logger.LogInformation("Client {ConnectionId} đã tham gia group ChildId={ChildId}", Context.ConnectionId, childId);
        }

        // 🔹 Rời nhóm
        public async Task LeaveChildGroup(Guid childId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, childId.ToString());
            _logger.LogInformation("Client {ConnectionId} đã rời group ChildId={ChildId}", Context.ConnectionId, childId);
        }

        // 🔹 Gửi lệnh từ phụ huynh tới thiết bị (VD: khoá máy)
        public async Task SendLockCommand(Guid childId)
        {
            _logger.LogInformation("Phụ huynh gửi lệnh khóa thiết bị của ChildId={ChildId}", childId);
            await Clients.Group(childId.ToString()).SendAsync("DeviceLocked", childId);
        }

        // 🔹 Gửi lệnh mở khóa
        public async Task SendUnlockCommand(Guid childId)
        {
            _logger.LogInformation("Phụ huynh gửi lệnh mở khóa thiết bị của ChildId={ChildId}", childId);
            await Clients.Group(childId.ToString()).SendAsync("DeviceUnlocked", childId);
        }

        // 🔹 Thiết bị gửi cập nhật trạng thái (pin, online,...)
        public async Task UpdateDeviceStatus(Guid childId, int pin, bool online)
        {
            _logger.LogInformation("Thiết bị cập nhật trạng thái: ChildId={ChildId}, Pin={Pin}, Online={Online}", childId, pin, online);

            await Clients.Group(childId.ToString()).SendAsync("DeviceStatusUpdated", new
            {
                ChildId = childId,
                Pin = pin,
                Online = online,
                Timestamp = DateTime.Now
            });
        }
    }
}
