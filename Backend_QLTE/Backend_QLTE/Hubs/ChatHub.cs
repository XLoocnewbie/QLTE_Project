using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Backend_QLTE.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChildDbContext _childDbContext;
        private readonly ILogger<ChatHub> _logger;
        private readonly IUserRepository _userRepository;
        public ChatHub(ChildDbContext childDbContext, IUserRepository userRepository ,ILogger<ChatHub> logger)
        {
            _childDbContext = childDbContext;
            _userRepository = userRepository;
            _logger = logger;
        }

        // Khi FE kết nối, thêm vào group riêng theo userId
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("{UserId} connected, connectionId={ConnectionId}", userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("{UserId} disconnected", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Gửi tin nhắn riêng cho 1 user
        public async Task<Guid> SendPrivateMessage(string senderId, string receiverId, string message)
        {
            var sender = await _userRepository.FindByUserIdAsync(senderId);
            var receiver = await _userRepository.FindByUserIdAsync(receiverId);
            if (sender != null && receiver != null)
            {
                var msg = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    NoiDung = message,
                    ThoiGian = DateTime.Now,
                    LoaiTinNhan = "text",
                    DaDoc = false
                };

                // 🔹 Lưu DB
                var add = await _childDbContext.ChatMessages.AddAsync(msg);
                await _childDbContext.SaveChangesAsync();

                _logger.LogInformation("Private message {Id} from {Sender} -> {Receiver}: {Message}",msg.MessageId, senderId, receiverId, message);
                // FE receiver sẽ nhận được event "ReceivePrivateMessage"
                await Clients.Group(receiverId).SendAsync("ReceivePrivateMessage", msg.MessageId, senderId, message);
                return msg.MessageId;
            }
            throw new ApiException("Gửi hoặc nhận không tồn tại", 404); 
        }

        // Đánh dấu tin nhắn đã đọc
        public async Task MarkMessageAsRead(Guid messageId, string readerId)
        {
            var message = await _childDbContext.ChatMessages.FindAsync(messageId);
            if (message == null)
            {
                _logger.LogWarning("Message {MessageId} not found", messageId);
                return;
            }

            // Kiểm tra đúng người nhận mới được phép đánh dấu đã đọc
            if (message.ReceiverId != readerId)
            { 
                _logger.LogWarning("User {UserId} tried to mark message {MessageId} not belonging to them", readerId, messageId);
                return;
            }

            message.DaDoc = true;
            await _childDbContext.SaveChangesAsync();

            _logger.LogInformation("Message {MessageId} marked as read by {ReaderId}", messageId, readerId);

            // Gửi thông báo lại cho người gửi biết tin nhắn đã được đọc
            await Clients.Group(message.SenderId)
                .SendAsync("MessageRead", messageId, readerId);
        }

        public async Task SendOffer(string fromUserId, string toUserId, string sdp, string type)
        {
            await Clients.Group(toUserId).SendAsync("ReceiveOffer", fromUserId, sdp, type);
        }

        // Gửi answer
        public async Task SendAnswer(string fromUserId, string toUserId, string sdp)
        {
            await Clients.Group(toUserId).SendAsync("ReceiveAnswer", fromUserId, sdp);
        }

        // Gửi ICE candidate
        public async Task SendIce(string fromUserId, string toUserId, object ice)
        {
            await Clients.Group(toUserId).SendAsync("ReceiveIceCandidate", fromUserId, ice);
        }
    }
}
