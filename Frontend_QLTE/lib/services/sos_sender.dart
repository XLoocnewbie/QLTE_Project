import 'package:geolocator/geolocator.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:frontend_qlte/services/sos_request_service.dart';

// Pattern giống như senior đã làm ở LocationAutoSender,
class SOSSender {
  final SignalRService _signalR = SignalRService();

  /// Hàm gửi tín hiệu SOS: gọi API -> phát realtime
  Future<Map<String, dynamic>> sendSOS() async {
    try {
      print("🚀 Bắt đầu quy trình gửi SOS...");

      // 🛰️ Lấy vị trí hiện tại của thiết bị
      final position = await Geolocator.getCurrentPosition(
        desiredAccuracy: LocationAccuracy.high,
      );
      print("📍 Lấy vị trí thành công: (${position.latitude}, ${position.longitude})");

      // 🟢 Gửi yêu cầu SOS đến API backend
      final result = await SOSRequestService.createSOSRequest(
        viDo: position.latitude,
        kinhDo: position.longitude,
        thoiGian: DateTime.now(),
      );

      // 🟢 Nếu API thành công → phát tín hiệu realtime
      if (result["success"] == true) {
        print("✅ API tạo SOS thành công. Tiến hành phát tín hiệu realtime...");

        await _signalR.connectSOS();
        await _signalR.sendSOSRealtime(
          "📢 SOS mới được gửi từ tọa độ (${position.latitude}, ${position.longitude})",
        );

        print("📡 SOS realtime signal sent thành công qua SOSHub");
      } else {
        print("⚠️ Gửi SOS thất bại: ${result["message"]}");
      }

      return result;
    } catch (e) {
      print("❌ Lỗi trong quá trình gửi SOS: $e");
      return {"success": false, "message": e.toString()};
    }
  }
}
