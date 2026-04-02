import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/study_period_model.dart';
import 'package:frontend_qlte/services/device_restriction_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class StudyPeriodService {
  static final String _baseUrl = '${Config_URL.baseUrl}StudyPeriod';

  static Map<String, dynamic>? _safeDecode(String body) {
    try {
      return jsonDecode(body);
    } catch (_) {
      return null;
    }
  }

  /// 🟢 Lấy danh sách StudyPeriod theo Child
  static Future<Map<String, dynamic>> getByChild(String childId,
      {int page = 1, int limit = 10}) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final response = await http.get(
        Uri.parse(
            '$_baseUrl/GetAllByChild?childId=$childId&page=$page&limit=$limit'),
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        final data = json?['data'] as List;
        final list = data.map((e) => StudyPeriod.fromJson(e)).toList();
        return {"success": true, "data": list};
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không thể tải danh sách (${response.statusCode})"
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🔵 Lấy chi tiết khung giờ học
  static Future<Map<String, dynamic>> getDetail(String studyPeriodId) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final response = await http.get(
        Uri.parse('$_baseUrl/Detail/$studyPeriodId'),
        headers: {"Authorization": "Bearer $token"},
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {
          "success": true,
          "data": StudyPeriod.fromJson(json!['data']),
        };
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không tìm thấy khung giờ học"
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🟡 Tạo mới khung giờ học
  static Future<Map<String, dynamic>> create({
    required String childId,
    required String startTime,
    required String endTime,
    String? moTa,
    String repeatPattern = "Daily",
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final body = jsonEncode({
        "childId": childId,
        "startTime": startTime,
        "endTime": endTime,
        "moTa": moTa,
        "repeatPattern": repeatPattern,
      });

      final url = Uri.parse('$_baseUrl/Create');
      print("📡 [POST] $url");
      print("📦 Body: $body");
      print("🔑 Token: ${token.substring(0, 15)}...");

      final response = await http.post(
        url,
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
        body: body,
      );

      print("📥 Status: ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {"success": true, "message": json?['msg'] ?? "Tạo thành công"};
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không thể tạo (${response.statusCode})"
      };
    } catch (e) {
      print("❌ Lỗi tạo StudyPeriod: $e");
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟠 Cập nhật khung giờ học
  static Future<Map<String, dynamic>> update({
    required String studyPeriodId,
    required String startTime,
    required String endTime,
    String? moTa,
    String repeatPattern = "Daily",
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final body = jsonEncode({
        "studyPeriodId": studyPeriodId,
        "startTime": startTime,
        "endTime": endTime,
        "moTa": moTa,
        "repeatPattern": repeatPattern,
      });

      final response = await http.put(
        Uri.parse('$_baseUrl/Update'),
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
        body: body,
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {"success": true, "message": json?['msg'] ?? "Cập nhật thành công"};
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không thể cập nhật (${response.statusCode})"
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🔴 Xóa khung giờ học
  static Future<Map<String, dynamic>> delete(String studyPeriodId) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final response = await http.delete(
        Uri.parse('$_baseUrl/Delete?studyPeriodId=$studyPeriodId'),
        headers: {"Authorization": "Bearer $token"},
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {"success": true, "message": json?['msg'] ?? "Xóa thành công"};
      }
      return {
        "success": false,
        "message": json?['msg'] ?? "Không thể xóa (${response.statusCode})"
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟣 Bật / tắt khung giờ học (ToggleActive)
  static Future<Map<String, dynamic>> toggleActive(String studyPeriodId) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final url = Uri.parse('$_baseUrl/ToggleActive?studyPeriodId=$studyPeriodId');
      print("📡 [PATCH] $url");
      print("🔑 Token: ${token.substring(0, 15)}...");

      final response = await http.patch(
        url,
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
      );

      print("📥 Status: ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        // ✅ data có thể chứa BlockedApps, Message, IsActive
        return {
          "success": true,
          "message": json?['message'] ?? json?['msg'] ?? "Cập nhật trạng thái thành công",
          "data": json?['data']
        };
      }

      return {
        "success": false,
        "message": json?['message'] ?? json?['msg'] ?? "Không thể cập nhật (${response.statusCode})"
      };
    } catch (e) {
      print("❌ Lỗi ToggleActive: $e");
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Lấy khung giờ học đang hoạt động của 1 đứa trẻ (Child)
  static Future<Map<String, dynamic>> getActiveByChild(String childId) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final url = Uri.parse("$_baseUrl/GetActiveByChild?childId=$childId");
      print("📡 [GET] $url");
      print("🔑 Token: ${token.substring(0, 15)}...");

      final response = await http.get(
        url,
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
      );

      print("📥 Status: ${response.statusCode}");
      print("📥 Body: ${response.body}");

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        final data = json?['data'];
        if (data != null) {
          return {
            "success": true,
            "message": json?['msg'] ?? "Đang trong khung giờ học.",
            "data": StudyPeriod.fromJson(data)
          };
        }
      }

      return {
        "success": false,
        "message": json?['msg'] ?? "Không có khung giờ học đang hoạt động.",
        "data": null
      };
    } catch (e) {
      print("❌ Lỗi khi lấy khung giờ học hiện tại: $e");
      return {"success": false, "message": "Lỗi kết nối: $e", "data": null};
    }
  }
}
