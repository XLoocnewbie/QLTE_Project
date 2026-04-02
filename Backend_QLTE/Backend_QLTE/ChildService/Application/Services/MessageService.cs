using Backend_QLTE.ChildService.Application.DTOs.Client.Message;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class MessageService : IMessageService
    { 
        private readonly ChildDbContext _childDbContext;
        private readonly IUserRepository _userRepository;
        public MessageService(ChildDbContext childDbContext, IUserRepository userRepository)
        {
            _childDbContext = childDbContext;
            _userRepository = userRepository;
        }

        public async Task<ResultListDTO<MessageResponseDTO>> GetLastMessagePerUserAsync(GetLastMessageUserRequestDTO request)
        {
            var user = await _userRepository.FindByUserIdAsync(request.UserId);
            if (user == null)
            {
                throw new ApiException($"Người dùng '{request.UserId}' không tồn tại",404);
            }

            // Lấy tất cả tin nhắn liên quan đến user
            var userMessages = await _childDbContext.ChatMessages
                .Where(m => m.SenderId == request.UserId || m.ReceiverId == request.UserId)
                .ToListAsync();

            // Nhóm theo người còn lại (OtherUserID)
            var latestMessages = userMessages
                .GroupBy(m => m.SenderId == request.UserId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.ThoiGian).First())
                .ToList();

            if(latestMessages.Count == 0)
            {
                throw new ApiException("Không có tin nhắn nào với người dùng nào",404);
            }

            var listMsg = latestMessages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                NoiDung = m.NoiDung,
                ThoiGian = m.ThoiGian,
                LoaiTinNhan = m.LoaiTinNhan,
                DaDoc = m.DaDoc
            }).ToList();

            return ResultListDTO<MessageResponseDTO>.Success(listMsg, "Lấy danh sách tin nhắn với bạn thành công");
        }


        public async Task<ResultListDTO<MessageDTO>> GetHistoryAsync(string user1Id, string user2Id, int page, int pageSize)
        {
            // Lấy danh sách tin nhắn giữa 2 người
            var msg = await _childDbContext.ChatMessages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.ThoiGian)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if(msg.Count == 0)
            {
                throw new ApiException("Không có lịch sử tin nhắn nào", 404);
            }
            // Lấy thông tin người dùng (để đổ vào DTO)
            var user1 = await _userRepository.FindByUserIdAsync(user1Id);
            var user2 = await _userRepository.FindByUserIdAsync(user2Id);

            // Chuyển sang DTO
            var listMsg = msg.Select(m => new MessageDTO
            {
                MessageID = m.MessageId,
                Content = m.NoiDung,
                Seen = m.DaDoc,
                Type = m.LoaiTinNhan,
                Timestamp = m.ThoiGian,

                FromUser = m.SenderId == user1Id
                    ? new UserShortDto
                    {
                        Id = user1!.Id,
                        FullName = user1.NameND,
                        Avatar = user1.AvatarND,
                        UserName = user1.UserName
                    }
                    : new UserShortDto
                    {
                        Id = user2!.Id,
                        FullName = user2.NameND,
                        Avatar = user2.AvatarND,
                        UserName = user2.UserName
                    },

                ToUser = m.ReceiverId == user1Id
                    ? new UserShortDto
                    {
                        Id = user1!.Id,
                        FullName = user1.NameND,
                        Avatar = user1.AvatarND,
                        UserName = user1.UserName
                    }
                    : new UserShortDto
                    {
                        Id = user2!.Id,
                        FullName = user2.NameND,
                        Avatar = user2.AvatarND,
                        UserName = user2.UserName
                    }
            }).ToList();

            // 4️⃣ Trả kết quả
            return new ResultListDTO<MessageDTO>
            {
                Status = true,
                Msg = "Lấy lịch sử tin nhắn thành công",
                Data = listMsg
            };
        }
        public async Task<int> CountMessagesAsync(string user1Id, string user2Id)
        {
            return await _childDbContext.ChatMessages
                .CountAsync(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                                 (m.SenderId == user2Id && m.ReceiverId == user1Id));
        }
    }
}

