using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend_QLTE.ChildService.Api.Hubs
{
    [Authorize] // 🔐 bắt buộc JWT
    public class ChatHub : Hub
    {
    }
}
