using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Backend_QLTE.ChildService.Domain.Services
{
    public class DeviceInfoDomainService : IDeviceInfoDomainService
    {
        public void EnsureCanCreate(DeviceInfo deviceInfo)
        {
            Validate(deviceInfo);
        }

        public void EnsureCanUpdate(DeviceInfo deviceInfo)
        {
            if (deviceInfo is null)
                throw new DeviceInfoNotFoundException("Không tìm thấy thiết bị để cập nhật.");

            Validate(deviceInfo);
        }

        public void EnsureCanDelete(DeviceInfo deviceInfo)
        {
            if (deviceInfo is null)
                throw new DeviceInfoNotFoundException("Không tìm thấy thiết bị để xóa.");
        }

        // 🔍 Hàm kiểm tra logic nghiệp vụ DeviceInfo
        private static void Validate(DeviceInfo deviceInfo)
        {
            if (deviceInfo == null)
                throw new InvalidDeviceInfoException("Dữ liệu thiết bị không được null.");

            if (deviceInfo.ChildId == Guid.Empty)
                throw new InvalidDeviceInfoException("Thiếu thông tin liên kết trẻ em (ChildId).");

            if (string.IsNullOrWhiteSpace(deviceInfo.TenThietBi))
                throw new InvalidDeviceInfoException("Tên thiết bị không được để trống.");

            if (deviceInfo.TenThietBi.Length > 100)
                throw new InvalidDeviceInfoException("Tên thiết bị vượt quá 100 ký tự.");

            if (string.IsNullOrWhiteSpace(deviceInfo.IMEI))
                throw new InvalidDeviceInfoException("IMEI không được để trống.");

            if (deviceInfo.IMEI.Length < 10 || deviceInfo.IMEI.Length > 50)
                throw new InvalidDeviceInfoException("Độ dài IMEI phải từ 10 đến 50 ký tự.");

            if (!Regex.IsMatch(deviceInfo.IMEI, @"^[a-zA-Z0-9]+$"))
                throw new InvalidDeviceInfoException("IMEI chỉ được chứa ký tự chữ và số.");

            if (deviceInfo.Pin is < 0 or > 100)
                throw new InvalidDeviceInfoException("Mức pin phải nằm trong khoảng từ 0 đến 100%.");

            if (deviceInfo.LanCapNhatCuoi > DateTime.Now.AddMinutes(2))
                throw new InvalidDeviceInfoException("Thời gian cập nhật cuối không thể ở tương lai.");

            // ⚡ Logic mới cho IsTracking & IsLocked
            if (deviceInfo.IsLocked && deviceInfo.IsTracking)
                throw new InvalidDeviceInfoException("Thiết bị không thể vừa bị khóa vừa trong trạng thái theo dõi.");

            if (!deviceInfo.IsTracking && !deviceInfo.IsLocked)
            {
                // Đây là trạng thái "trung tính", có thể cảnh báo hoặc cho phép tùy nghiệp vụ
                // Chúng ta chỉ log cảnh báo chứ không throw error
                Console.WriteLine("⚠️ Cảnh báo: Thiết bị đang ở trạng thái không theo dõi và không bị khóa.");
            }
        }
    }
}
