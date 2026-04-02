import 'dart:convert';
import 'package:http/http.dart' as http;

class HttpResponseHelper {
  /// ✅ Chuẩn hóa response trả về từ backend
  static Map<String, dynamic> handle(http.Response response) {
    print("📡 HTTP ${response.request?.method} → ${response.statusCode}");
    print("📥 Response body: ${response.body}");

    try {
      final decoded = jsonDecode(response.body);

      // 🟢 Nếu có status (chuẩn ASP.NET API)
      if (decoded is Map && decoded.containsKey("status")) {
        final bool status = decoded["status"] ?? false;
        return {
          "success": status,
          "message": decoded["msg"] ?? "Không rõ thông báo.",
          "data": decoded["data"]
        };
      }

      // 🟡 Nếu không có field status (backend trả raw JSON)
      return {
        "success": response.statusCode == 200,
        "message": "Phản hồi thành công (HTTP ${response.statusCode})",
        "data": decoded,
      };
    } catch (e) {
      // 🔴 Khi body không phải JSON
      return {
        "success": response.statusCode == 200,
        "message": "Phản hồi không phải JSON (HTTP ${response.statusCode})",
        "data": response.body
      };
    }
  }

  /// ✅ Helper kiểm tra theo status code
  static Map<String, dynamic> handleStatus(http.Response response) {
    switch (response.statusCode) {
      case 200:
      case 201:
        return handle(response);
      case 400:
        return {"success": false, "message": "Dữ liệu gửi lên không hợp lệ (400)"};
      case 401:
        return {"success": false, "message": "Token hết hạn hoặc không hợp lệ (401)"};
      case 403:
        return {"success": false, "message": "Bạn không có quyền truy cập (403)"};
      case 404:
        return {"success": false, "message": "Không tìm thấy tài nguyên (404)"};
      case 409:
        return {"success": false, "message": "Dữ liệu bị trùng lặp (409)"};
      case 500:
        return {"success": false, "message": "Lỗi hệ thống, vui lòng thử lại (500)"};
      default:
        return {"success": false, "message": "Lỗi không xác định (HTTP ${response.statusCode})"};
    }
  }
}
