using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Backend_QLTE.Hubs
{
    public class DeviceHub : Hub
    {
        private readonly ILogger<DeviceHub> _logger;

        public DeviceHub(ILogger<DeviceHub> logger)
        {
            _logger = logger;
        }

        // ✅ Khi client (Parent/Child) kết nối vào Hub
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("📡 Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // ✅ Khi client ngắt kết nối khỏi Hub
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("🔌 Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Cho phép client join group theo ChildId
        public async Task JoinGroup(string childId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, childId);
            _logger.LogInformation("👥 Client {ConnectionId} joined group {GroupId}", Context.ConnectionId, childId);
        }

        // ✅ Cho phép client rời group
        public async Task LeaveGroup(string childId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, childId);
            _logger.LogInformation("🚪 Client {ConnectionId} left group {GroupId}", Context.ConnectionId, childId);
        }

        // 🧪 Hàm test gửi tin nhắn realtime (Dev test)
        public async Task SendTest(string message)
        {
            _logger.LogInformation("🧪 SendTest: broadcasting message = {Message}", message);
            await Clients.All.SendAsync("DeviceTest", new
            {
                Message = message,
                Time = DateTime.Now
            });
        }
    }
}
