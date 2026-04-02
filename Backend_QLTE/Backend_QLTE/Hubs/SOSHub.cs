using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Backend_QLTE.Hubs
{
    public class SOSHub : Hub
    {
        private readonly ILogger<SOSHub> _logger;

        public SOSHub(ILogger<SOSHub> logger)
        {
            _logger = logger;
        }

        // ✅ Khi client (Parent/Child/Admin) kết nối vào Hub
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

        // ✅ Cho phép client join group theo ChildId hoặc ParentId (để gửi riêng từng nhóm)
        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            _logger.LogInformation("👥 Client {ConnectionId} joined group {GroupId}", Context.ConnectionId, groupId);
        }

        // ✅ Cho phép client rời group
        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            _logger.LogInformation("🚪 Client {ConnectionId} left group {GroupId}", Context.ConnectionId, groupId);
        } 

        // 🧪 Hàm test gửi tin nhắn realtime (Admin/Dev test)
        public async Task SendTest(string message)
        {
            _logger.LogInformation("🧪 SendTest: broadcasting message = {Message}", message);
            await Clients.All.SendAsync("ReceiveSOS", new
            {
                Message = message,
                Time = DateTime.Now
            });
        }
    }
}
