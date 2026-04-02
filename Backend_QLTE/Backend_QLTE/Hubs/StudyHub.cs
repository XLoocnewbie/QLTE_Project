using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Backend_QLTE.Hubs
{
    public class StudyHub : Hub
    {
        private readonly ILogger<StudyHub> _logger;

        public StudyHub(ILogger<StudyHub> logger)
        {
            _logger = logger;
        }

        // ✅ Khi client (Parent/Child) kết nối
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("📡 [StudyHub] Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // ✅ Khi client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("🔌 [StudyHub] Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Cho phép client join group theo ChildId
        public async Task JoinGroup(string childId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, childId);
            _logger.LogInformation("👥 [StudyHub] Client {ConnectionId} joined group {GroupId}", Context.ConnectionId, childId);
        }

        // ✅ Cho phép client rời group
        public async Task LeaveGroup(string childId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, childId);
            _logger.LogInformation("🚪 [StudyHub] Client {ConnectionId} left group {GroupId}", Context.ConnectionId, childId);
        }

        // 🧪 Hàm test realtime (giúp dev kiểm tra kết nối)
        public async Task SendTest(string message)
        {
            _logger.LogInformation("🧪 [StudyHub] Broadcasting test message: {Message}", message);
            await Clients.All.SendAsync("StudyTest", new
            {
                Message = message,
                Time = DateTime.Now
            });
        }
    }
}
