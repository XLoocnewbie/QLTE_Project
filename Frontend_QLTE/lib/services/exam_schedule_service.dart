import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/exam_schedule_model.dart';

class ExamScheduleService {
  static final String _baseUrl = '${Config_URL.baseUrl}ExamSchedule';

  /// 🧩 Helper: parse JSON an toàn
  static Map<String, dynamic>? _safeDecode(String body) {
    try {
      if (body.isEmpty) return null;
      return jsonDecode(body);
    } catch (_) {
      return null;
    }
  }

  /// 🟢 Lấy tất cả lịch thi theo trẻ
  static Future<Map<String, dynamic>> getAllByChild(String childId, String token) async {
    try {
      final response = await http.get(
        Uri.parse('$_baseUrl/GetAllByChild?childId=$childId'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );

      final json = _safeDecode(response.body);
      if (response.statusCode == 200 && json?['status'] == true && json?['data'] is List) {
        List<ExamSchedule> list =
        (json!['data'] as List).map((e) => ExamSchedule.fromJson(e)).toList();
        return {"success": true, "data": list};
      }

      return {
        "success": false,
        "message": json?['msg'] ?? "Không có dữ liệu lịch thi (${response.statusCode})",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Tạo mới lịch thi
  static Future<Map<String, dynamic>> createExamSchedule({
    required String childId,
    required String monThi,
    required DateTime thoiGianThi,
    String? ghiChu,
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
          "monThi": monThi,
          "thoiGianThi": thoiGianThi.toIso8601String(),
          "ghiChu": ghiChu,
        }),
      );

      // ⚙️ Chấp nhận 200 / 201 / 204 là thành công
      if ([200, 201, 204].contains(response.statusCode)) {
        final json = _safeDecode(response.body);
        if (json == null) {
          // body rỗng → vẫn coi là success
          return {"success": true, "message": "Tạo lịch thi thành công"};
        }
        return {
          "success": json["status"] ?? true,
          "message": json["msg"] ?? "Tạo lịch thi thành công",
        };
      }

      // Nếu mã lỗi khác
      final json = _safeDecode(response.body);
      return {
        "success": false,
        "message": json?["msg"] ?? "Lỗi hệ thống (${response.statusCode})",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🟡 Cập nhật lịch thi
  static Future<Map<String, dynamic>> updateExamSchedule({
    required String examId,
    required String monThi,
    required DateTime thoiGianThi,
    String? ghiChu,
    required bool daThiXong,
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
          "examId": examId,
          "monThi": monThi,
          "thoiGianThi": thoiGianThi.toIso8601String(),
          "ghiChu": ghiChu,
          "daThiXong": daThiXong,
        }),
      );

      final json = _safeDecode(response.body);
      if ([200, 204].contains(response.statusCode)) {
        return {
          "success": json?["status"] ?? true,
          "message": json?["msg"] ?? "Cập nhật thành công",
        };
      }

      return {
        "success": false,
        "message": json?["msg"] ?? "Không thể cập nhật (${response.statusCode})",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🔴 Xóa lịch thi
  static Future<Map<String, dynamic>> deleteExamSchedule(String examId, String token) async {
    try {
      final response = await http.delete(
        Uri.parse('$_baseUrl/Delete?ExamId=$examId'),
        headers: {"Authorization": "Bearer $token"},
      );

      final json = _safeDecode(response.body);
      if ([200, 204].contains(response.statusCode)) {
        return {
          "success": json?["status"] ?? true,
          "message": json?["msg"] ?? "Xoá thành công",
        };
      }

      return {
        "success": false,
        "message": json?["msg"] ?? "Không thể xoá (${response.statusCode})",
      };
    } catch (e) {
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }
}
