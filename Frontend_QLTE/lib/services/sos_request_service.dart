import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/sos_request_model.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SOSRequestService {
  static final String _baseUrl = '${Config_URL.baseUrl}SOSRequest';

  /// 🧩 Helper: parse JSON an toàn
  static Map<String, dynamic>? _safeDecode(String body) {
    try {
      if (body.isEmpty) return null;
      return jsonDecode(body);
    } catch (_) {
      return null;
    }
  }

  /// 🟢 Lấy tất cả SOSRequest (Admin)
  static Future<Map<String, dynamic>> getAll(String token, {int page = 1, int limit = 10}) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/GetAll?page=$page&limit=$limit'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true && json?['data'] is List) {
        final list = (json!['data'] as List).map((e) => SOSRequest.fromJson(e)).toList();
        return {"success": true, "data": list};
      }

      return {"success": false, "message": json?['msg'] ?? "Không thể tải dữ liệu (${response.statusCode})"};
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Lấy danh sách SOSRequest theo ChildId
  static Future<Map<String, dynamic>> getByChild(String childId, String token,
      {int page = 1, int limit = 10}) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/GetByChild?childId=$childId&page=$page&limit=$limit'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true && json?['data'] is List) {
        final list = (json!['data'] as List).map((e) => SOSRequest.fromJson(e)).toList();
        return {"success": true, "data": list};
      }

      return {"success": false, "message": json?['msg'] ?? "Không có dữ liệu (${response.statusCode})"};
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🔵 Lấy chi tiết SOSRequest
  static Future<Map<String, dynamic>> getDetail(String sosId, String token) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/Detail/$sosId'),
        headers: {
          "Authorization": "Bearer $token",
        },
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true) {
        return {
          "success": true,
          "data": SOSRequest.fromJson(json!['data']),
        };
      }

      return {"success": false, "message": json?['msg'] ?? "Không tìm thấy dữ liệu"};
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Gửi tín hiệu SOS (Children)
  static Future<Map<String, dynamic>> createSOSRequest({
    required double viDo,
    required double kinhDo,
    required DateTime thoiGian,
  }) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('jwt_token');
      final childId = prefs.getString('child_id');

      if (token == null || token.isEmpty) {
        throw Exception("Không tìm thấy token. Vui lòng đăng nhập lại!");
      }
      if (childId == null || childId.isEmpty) {
        throw Exception("Không tìm thấy ChildId trong dữ liệu lưu trữ!");
      }

      print("🚨 Gửi SOS cho ChildId: $childId");

      final response = await http.post(
        Uri.parse('$_baseUrl/Create'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
        body: jsonEncode({
          "childId": childId,
          "viDo": viDo,
          "kinhDo": kinhDo,
          "thoiGian": thoiGian.toIso8601String(),
        }),
      );

      print("📥 SOS Response: ${response.statusCode} - ${response.body}");

      final json = _safeDecode(response.body);
      if ([200, 201, 204].contains(response.statusCode)) {
        return {
          "success": json?['status'] ?? true,
          "message": json?['msg'] ?? "Gửi tín hiệu SOS thành công",
        };
      }

      return {"success": false, "message": json?['msg'] ?? "Không thể gửi SOS (${response.statusCode})"};
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng hoặc dữ liệu: $e"};
    }
  }

  /// 🟠 Cập nhật SOSRequest (chỉ Admin)
  static Future<Map<String, dynamic>> updateSOSRequest({
    required String sosId,
    required String trangThai,
    required String token,
  }) async {
    try {
      final response = await http.put(
        Uri.parse('$_baseUrl/Update'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
        body: jsonEncode({
          "sosId": sosId,
          "trangThai": trangThai,
        }),
      );

      final json = _safeDecode(response.body);
      if ([200, 204].contains(response.statusCode)) {
        return {
          "success": json?['status'] ?? true,
          "message": json?['msg'] ?? "Cập nhật thành công",
        };
      }

      return {"success": false, "message": json?['msg'] ?? "Không thể cập nhật (${response.statusCode})"};
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🔴 Xóa SOSRequest (Admin)
  static Future<Map<String, dynamic>> deleteSOSRequest(String sosId, String token) async {
    try {
      final response = await http.delete(
        Uri.parse('$_baseUrl/Delete?sosId=$sosId'),
        headers: {
          "Authorization": "Bearer $token",
        },
      );

      final json = _safeDecode(response.body);
      if ([200, 204].contains(response.statusCode)) {
        return {
          "success": json?['status'] ?? true,
          "message": json?['msg'] ?? "Xóa yêu cầu SOS thành công",
        };
      }

      return {"success": false, "message": json?['msg'] ?? "Không thể xóa (${response.statusCode})"};
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }
}
