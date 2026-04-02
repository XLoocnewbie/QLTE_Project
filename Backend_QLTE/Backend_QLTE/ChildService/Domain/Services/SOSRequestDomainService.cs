using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;

namespace Backend_QLTE.ChildService.Domain.Services
{
    public class SOSRequestDomainService : ISOSRequestDomainService
    {
        public void EnsureCanCreate(SOSRequest sosRequest, IEnumerable<SOSRequest> existingRequests)
        {
            Validate(sosRequest);

            // 🔁 Kiểm tra xem đã có SOS nào "đang xử lý" cho cùng đứa trẻ chưa
            if (existingRequests.Any(r => r.TrangThai == "Đang xử lý"))
                throw new DuplicateSOSRequestException(sosRequest.ChildId, sosRequest.ThoiGian);
        }

        public void EnsureCanUpdate(SOSRequest sosRequest)
        {
            if (sosRequest is null)
                throw new SOSRequestNotFoundException(Guid.Empty);

            Validate(sosRequest);
        }

        public void EnsureCanDelete(SOSRequest sosRequest)
        {
            if (sosRequest is null)
                throw new SOSRequestNotFoundException(Guid.Empty);
        }

        private static void Validate(SOSRequest sosRequest)
        {
            if (sosRequest is null)
                throw new InvalidSOSRequestException("Dữ liệu yêu cầu SOS không được null.");

            // 📅 Không cho phép thời gian ở tương lai
            if (sosRequest.ThoiGian > DateTime.Now.AddMinutes(2))
                throw new InvalidSOSRequestException("Thời gian gửi yêu cầu không hợp lệ (ở tương lai).");

            // 📍 Kiểm tra tọa độ
            if (sosRequest.ViDo < -90 || sosRequest.ViDo > 90)
                throw new InvalidSOSRequestException("Vĩ độ (Latitude) phải nằm trong khoảng -90 đến 90.");

            if (sosRequest.KinhDo < -180 || sosRequest.KinhDo > 180)
                throw new InvalidSOSRequestException("Kinh độ (Longitude) phải nằm trong khoảng -180 đến 180.");

            // ⚙️ Kiểm tra trạng thái hợp lệ
            var validStatuses = new[] { "Đang xử lý", "Đã xử lý", "Đã hủy" };
            if (!validStatuses.Contains(sosRequest.TrangThai))
                throw new InvalidSOSRequestException($"Trạng thái '{sosRequest.TrangThai}' không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validStatuses)}.");
        }
    }
}
