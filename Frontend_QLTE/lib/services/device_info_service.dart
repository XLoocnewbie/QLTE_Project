import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/device_info_model.dart';

class DeviceInfoService {
  static final String _baseUrl = '${Config_URL.baseUrl}DeviceInfo';

  static Map<String, dynamic>? _safeDecode(String body) {
    try {
      if (body.isEmpty) return null;
      return jsonDecode(body);
    } catch (_) {
      return null;
    }
  }

  /// 🟢 Lấy tất cả thiết bị (Admin hoặc Parent)
  static Future<Map<String, dynamic>> getAll(
      String token, {
        int page = 1,
        int limit = 10,
      }) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/GetAll?page=$page&limit=$limit'),
        headers: {"Authorization": "Bearer $token"},
      );

      print("📡 GetAll → ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        final list = (json!['data'] as List)
            .map((e) => DeviceInfoModel.fromJson(e))
            .toList();
        return {"success": true, "data": list};
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không thể tải dữ liệu thiết bị."
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🟡 Lấy danh sách thiết bị theo ChildId (cho Parent)
  static Future<Map<String, dynamic>> getByChild(
      String childId, String token) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/GetByChild?childId=$childId'),
        headers: {"Authorization": "Bearer $token"},
      );

      print("📡 GetByChild($childId) → ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        // 🔁 Cho phép backend trả 1 thiết bị hoặc nhiều thiết bị
        final data = json!['data'];
        if (data is List) {
          final list = data.map((e) => DeviceInfoModel.fromJson(e)).toList();
          return {"success": true, "data": list};
        } else {
          return {
            "success": true,
            "data": [DeviceInfoModel.fromJson(data)]
          };
        }
      }
      return {"success": false, "message": json?['msg'] ?? "Không có thiết bị nào."};
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🔵 Chi tiết thiết bị
  static Future<Map<String, dynamic>> getDetail(
      String deviceId, String token) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/Detail/$deviceId'),
        headers: {"Authorization": "Bearer $token"},
      );

      print("📡 GetDetail → ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {"success": true, "data": DeviceInfoModel.fromJson(json!['data'])};
      }
      return {"success": false, "message": json?['msg'] ?? "Không tìm thấy thiết bị."};
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🟢 Tạo mới thiết bị (Parent thêm thiết bị cho Child)
  static Future<Map<String, dynamic>> createDevice({
    required String childId,
    required String tenThietBi,
    required String imei,
    required String token,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$_baseUrl/Create'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
        body: jsonEncode({
          "childId": childId,
          "tenThietBi": tenThietBi,
          "imei": imei,
        }),
      );

      print("📡 CreateDevice → ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      return {
        "success": json?['status'] ?? false,
        "message": json?['msg'] ?? "Không thể tạo thiết bị.",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi tạo thiết bị: $e"};
    }
  }

  /// 🟠 Cập nhật trạng thái pin / online (child gửi định kỳ)
  static Future<void> updateStatus({
    required String deviceId,
    int? pin,
    bool? online,
  }) async {
    try {
      final url = Uri.parse(
          '$_baseUrl/UpdateStatus?deviceId=$deviceId&pin=$pin&online=$online');
      await http.put(url);
      print("📡 Cập nhật trạng thái thiết bị thành công!");
    } catch (e) {
      print("⚠️ Lỗi khi cập nhật trạng thái: $e");
    }
  }

  /// 🔒 Khoá thiết bị
  static Future<Map<String, dynamic>> lockDevice(
      String childId, String token) async {
    try {
      final response = await http.put(
        Uri.parse('$_baseUrl/LockDevice/$childId'),
        headers: {"Authorization": "Bearer $token"},
      );
      final json = _safeDecode(response.body);
      return {
        "success": json?['status'] ?? false,
        "message": json?['msg'] ?? "Không thể khoá thiết bị.",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🔓 Mở khoá thiết bị
  static Future<Map<String, dynamic>> unlockDevice(
      String childId, String token) async {
    try {
      final response = await http.put(
        Uri.parse('$_baseUrl/UnlockDevice/$childId'),
        headers: {"Authorization": "Bearer $token"},
      );
      final json = _safeDecode(response.body);
      return {
        "success": json?['status'] ?? false,
        "message": json?['msg'] ?? "Không thể mở khoá thiết bị.",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🆕 Bật / Tắt theo dõi định kỳ (IsTracking)
  static Future<Map<String, dynamic>> setTracking(
      String childId, bool isTracking, String token) async {
    try {
      final response = await http.put(
        Uri.parse('$_baseUrl/SetTracking/$childId?isTracking=$isTracking'),
        headers: {"Authorization": "Bearer $token"},
      );
      final json = _safeDecode(response.body);
      return {
        "success": json?['status'] ?? false,
        "message": json?['msg'] ?? "Không thể thay đổi trạng thái theo dõi.",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }

  /// 🔴 Xoá thiết bị
  static Future<Map<String, dynamic>> deleteDevice(
      String deviceId, String token) async {
    try {
      final response = await http.delete(
        Uri.parse('$_baseUrl/Delete?deviceId=$deviceId'),
        headers: {"Authorization": "Bearer $token"},
      );
      final json = _safeDecode(response.body);
      return {
        "success": json?['status'] ?? false,
        "message": json?['msg'] ?? "Không thể xoá thiết bị.",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi: $e"};
    }
  }
}
