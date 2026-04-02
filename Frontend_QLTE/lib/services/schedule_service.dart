import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import '../models/schedule_model.dart';

class ScheduleService {
  static final String _baseUrl = "${Config_URL.baseUrl}Schedule/";

  /// 🟢 Lấy tất cả lịch học theo ChildId
  static Future<Map<String, dynamic>> getAllByChild({
    required String childId,
    int page = 1,
    int limit = 10,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}GetAllByChild?childId=$childId&page=$page&limit=$limit");
    try {
      final response = await http.get(
        url,
        headers: {"Authorization": "Bearer $token"},
      );

      print("📥 [GET] GetAllByChild => ${response.statusCode}");
      print("📥 Response body: ${response.body}");

      final result = jsonDecode(response.body);
      if (result["status"] == true && result["data"] != null) {
        List<ScheduleModel> list = (result["data"] as List)
            .map((e) => ScheduleModel.fromJson(e))
            .toList();
        return {"success": true, "data": list};
      } else {
        return {
          "success": false,
          "message": result["msg"] ?? "Không có dữ liệu lịch học"
        };
      }
    } catch (e) {
      print("❌ Lỗi getAllByChild: $e");
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Lấy chi tiết 1 lịch học
  static Future<Map<String, dynamic>> getDetail({
    required String scheduleId,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}Detail/$scheduleId");
    try {
      final response = await http.get(url, headers: {"Authorization": "Bearer $token"});
      print("📥 [GET] GetDetail => ${response.statusCode}");

      final result = jsonDecode(response.body);
      if (result["status"] == true && result["data"] != null) {
        return {
          "success": true,
          "data": ScheduleModel.fromJson(result["data"]),
        };
      } else {
        return {"success": false, "message": result["msg"] ?? "Không tìm thấy lịch học"};
      }
    } catch (e) {
      print("❌ Lỗi getDetail: $e");
      return {"success": false, "message": "Lỗi kết nối: $e"};
    }
  }

  /// 🟢 Tạo mới lịch học
  static Future<Map<String, dynamic>> createSchedule({
    required String childId,
    required String tenMonHoc,
    required int thu,
    required String gioBatDau,
    required String gioKetThuc,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}Create");
    final body = jsonEncode({
      "childId": childId,
      "tenMonHoc": tenMonHoc,
      "thu": thu,
      "gioBatDau": gioBatDau,
      "gioKetThuc": gioKetThuc,
    });

    try {
      final response = await http.post(
        url,
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
        body: body,
      );

      print("📤 [POST] Create => ${response.statusCode}");
      print("📥 Response body: ${response.body}");

      final result = jsonDecode(response.body);
      return {
        "success": result["status"] ?? false,
        "message": result["msg"] ?? "Tạo lịch học thất bại",
      };
    } catch (e) {
      print("❌ Lỗi createSchedule: $e");
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🟡 Cập nhật lịch học
  static Future<Map<String, dynamic>> updateSchedule({
    required String scheduleId,
    required String childId,         // 🟢 Thêm dòng này
    required String tenMonHoc,
    required int thu,
    required String gioBatDau,
    required String gioKetThuc,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}Update");
    final body = jsonEncode({
      "scheduleId": scheduleId,
      "childId": childId,            // 🟢 Gửi kèm để backend ModelState hợp lệ
      "tenMonHoc": tenMonHoc,
      "thu": thu,
      "gioBatDau": gioBatDau,
      "gioKetThuc": gioKetThuc,
    });

    try {
      final response = await http.put(
        url,
        headers: {
          "Authorization": "Bearer $token",
          "Content-Type": "application/json",
        },
        body: body,
      );

      print("📤 [PUT] Update => ${response.statusCode}");
      print("📥 Response body: ${response.body}");

      final result = jsonDecode(response.body);
      return {
        "success": result["status"] ?? false,
        "message": result["msg"] ?? "Cập nhật lịch học thất bại",
      };
    } catch (e) {
      print("❌ Lỗi updateSchedule: $e");
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }

  /// 🔴 Xoá lịch học
  static Future<Map<String, dynamic>> deleteSchedule({
    required String scheduleId,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}Delete?ScheduleId=$scheduleId");
    try {
      final response = await http.delete(
        url,
        headers: {"Authorization": "Bearer $token"},
      );

      print("📤 [DELETE] Delete => ${response.statusCode}");
      print("📥 Response body: ${response.body}");

      final result = jsonDecode(response.body);
      return {
        "success": result["status"] ?? false,
        "message": result["msg"] ?? "Xoá lịch học thất bại",
      };
    } catch (e) {
      print("❌ Lỗi deleteSchedule: $e");
      return {"success": false, "message": "Lỗi mạng: $e"};
    }
  }
}
