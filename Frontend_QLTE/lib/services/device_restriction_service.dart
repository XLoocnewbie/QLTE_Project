import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/models/device_restriction_model.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:shared_preferences/shared_preferences.dart';

class DeviceRestrictionService {
  static final String _baseUrl = "${Config_URL.baseUrl}DeviceRestriction";

  /// 🧩 Hàm lấy token tiện ích
  static Future<Map<String, String>> _headers() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString("jwt_token") ?? "";
    return {
      "Authorization": "Bearer $token",
      "Content-Type": "application/json",
    };
  }

  // 🟢 Lấy danh sách cấu hình hạn chế theo DeviceId
  static Future<List<DeviceRestrictionModel>> getAllByDevice(String deviceId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/GetAllByDevice?deviceId=$deviceId");

    print("📡 [GET] $url");
    print("🔑 Headers: ${headers['Authorization']?.substring(0, 25)}...");

    final response = await http.get(url, headers: headers);

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    if (response.statusCode == 200) {
      final body = jsonDecode(response.body);
      if (body['status'] == true && body['data'] != null) {
        final List<dynamic> list = body['data'];
        print("✅ Nhận được ${list.length} cấu hình hạn chế từ server.");
        return list.map((e) => DeviceRestrictionModel.fromJson(e)).toList();
      } else {
        throw Exception(body['message'] ?? "Không có dữ liệu");
      }
    } else if (response.statusCode == 401 || response.statusCode == 403) {
      throw Exception("❌ Không có quyền truy cập hoặc token hết hạn.");
    } else {
      throw Exception("⚠️ Lỗi API: ${response.statusCode}");
    }
  }

  // 🔵 Lấy chi tiết cấu hình
  static Future<DeviceRestrictionModel> getDetail(String restrictionId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/GetDetail?restrictionId=$restrictionId");
    print("📡 GET $url");

    final response = await http.get(url, headers: headers);
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    if (response.statusCode == 200) {
      final body = jsonDecode(response.body);
      if (body['status'] == true && body['data'] != null) {
        return DeviceRestrictionModel.fromJson(body['data']);
      } else {
        throw Exception(body['message'] ?? "Không có dữ liệu");
      }
    } else {
      throw Exception("Lỗi API: ${response.statusCode}");
    }
  }

  // 🟡 Tạo mới cấu hình
  static Future<String> create(DeviceRestrictionModel model) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/Create");
    print("📡 POST $url");
    print("🧩 Body: ${jsonEncode(model.toJson())}");

    final response = await http.post(url, headers: headers, body: jsonEncode(model.toJson()));
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Tạo thành công";
    } else {
      throw Exception(body['message'] ?? "Lỗi tạo cấu hình");
    }
  }

  // 🟠 Cập nhật cấu hình
  static Future<String> update(DeviceRestrictionModel model) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/Update");
    print("📡 PUT $url");
    print("🧩 Body: ${jsonEncode(model.toJson())}");

    final response = await http.put(url, headers: headers, body: jsonEncode(model.toJson()));
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Cập nhật thành công";
    } else {
      throw Exception(body['message'] ?? "Lỗi cập nhật");
    }
  }

  // 🔴 Xoá cấu hình
  static Future<String> delete(String restrictionId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/Delete");
    print("📡 DELETE $url");

    final response = await http.delete(
      url,
      headers: headers,
      body: jsonEncode({"restrictionId": restrictionId}),
    );

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Xóa thành công";
    } else {
      throw Exception(body['message'] ?? "Lỗi xóa cấu hình");
    }
  }

  // 🟣 Bật / tắt Firewall
  static Future<String> toggleFirewall(String restrictionId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/ToggleFirewall?restrictionId=$restrictionId");
    print("📡 PATCH $url");

    final response = await http.patch(url, headers: headers);
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Cập nhật Firewall thành công";
    } else {
      throw Exception(body['message'] ?? "Lỗi bật/tắt Firewall");
    }
  }

  // 🟢 Kích hoạt chế độ học (Restriction)
  static Future<String> activateStudyRestriction(String deviceId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/ActivateStudyRestriction?deviceId=$deviceId");
    print("📡 POST $url");

    final response = await http.post(url, headers: headers);
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Đã bật chế độ học (Restriction)";
    } else {
      throw Exception(body['message'] ?? "Lỗi bật Restriction");
    }
  }

  // 🔴 Tắt chế độ học (Restriction)
  static Future<String> deactivateRestriction(String deviceId) async {
    final headers = await _headers();
    final url = Uri.parse("$_baseUrl/DeactivateRestriction?deviceId=$deviceId");
    print("📡 POST $url");

    final response = await http.post(url, headers: headers);
    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    final body = jsonDecode(response.body);
    if (response.statusCode == 200 && body['status'] == true) {
      return body['message'] ?? "Đã tắt Restriction";
    } else {
      throw Exception(body['message'] ?? "Lỗi tắt Restriction");
    }
  }
}
