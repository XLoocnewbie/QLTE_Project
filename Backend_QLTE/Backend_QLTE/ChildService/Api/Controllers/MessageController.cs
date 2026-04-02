using Backend_QLTE.ChildService.Application.DTOs.Client.Message;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetHistory(string user1Id, string user2Id, int page = 1, int pageSize = 20)
        {
            var messages = await _messageService.GetHistoryAsync(user1Id, user2Id, page, pageSize);
            var total = await _messageService.CountMessagesAsync(user1Id, user2Id);

            return Ok(new { total, page, pageSize, messages });
        }

        [HttpGet("GetLastMessagePerUser")]
        public async Task<IActionResult> GetLastMessagePerUser([FromQuery] GetLastMessageUserRequestDTO request)
        {
            var listMsg = await _messageService.GetLastMessagePerUserAsync(request);
            return Ok(listMsg);
        }
    }
}
