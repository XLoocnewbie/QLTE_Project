using Microsoft.AspNetCore.SignalR;

namespace Backend_QLTE.Hubs
{
    public class RestrictionHub : Hub
    {
        private readonly ILogger<RestrictionHub> _logger;

        public RestrictionHub(ILogger<RestrictionHub> logger)
        {
            _logger = logger;
        }

        // 🟢 Khi child kết nối → join group theo DeviceId
        public async Task JoinDeviceGroup(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("JoinDeviceGroup bị gọi với DeviceId rỗng");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"device-{deviceId}");
            _logger.LogInformation("✅ Device {DeviceId} đã join group device-{DeviceId}", deviceId, deviceId);

            await Clients.Caller.SendAsync("OnJoinedGroup", new
            {
                deviceId,
                message = "Kết nối RestrictionHub thành công."
            });
        }

        // 🔴 Khi client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client {ConnId} ngắt kết nối khỏi RestrictionHub", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // 🟣 Gửi thông báo thay đổi cấu hình hạn chế (create/update/delete)
        public async Task NotifyRestrictionChanged(string deviceId, object restrictionData)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("NotifyRestrictionChanged bị gọi với DeviceId rỗng");
                return;
            }

            _logger.LogInformation("📡 Gửi OnRestrictionChanged tới group device-{DeviceId}", deviceId);

            await Clients.Group($"device-{deviceId}")
                .SendAsync("OnRestrictionChanged", restrictionData);
        }

        // 🧩 Gửi realtime bật/tắt firewall cho app child
        public async Task SendFirewallStatus(string deviceId, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("SendFirewallStatus bị gọi với DeviceId rỗng");
                return;
            }

            var message = isEnabled ? "🔒 Firewall đã được bật" : "🔓 Firewall đã được tắt";

            await Clients.Group($"device-{deviceId}")
                .SendAsync("OnFirewallToggled", new
                {
                    deviceId,
                    isEnabled,
                    message,
                    time = DateTime.Now
                });

            _logger.LogInformation("📡 Gửi OnFirewallToggled={Status} tới device-{DeviceId}", isEnabled, deviceId);
        }

        // 🧠 Có thể bổ sung thêm để parent hoặc admin kiểm tra trạng thái realtime
        public async Task CheckConnectionStatus(string deviceId)
        {
            _logger.LogInformation("📍 Kiểm tra kết nối realtime cho device-{DeviceId}", deviceId);
            await Clients.Caller.SendAsync("OnCheckConnectionStatus", new
            {
                deviceId,
                connectedAt = DateTime.Now
            });
        }
    }
}
