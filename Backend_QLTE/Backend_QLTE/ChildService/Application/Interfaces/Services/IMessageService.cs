using Backend_QLTE.ChildService.Application.DTOs.Client.Message;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ResultListDTO<MessageResponseDTO>> GetLastMessagePerUserAsync(GetLastMessageUserRequestDTO request);
        Task<ResultListDTO<MessageDTO>> GetHistoryAsync(string user1Id, string user2Id, int page, int pageSize);
        Task<int> CountMessagesAsync(string user1Id, string user2Id);
    }
}
