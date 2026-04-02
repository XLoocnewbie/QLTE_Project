import 'dart:convert';

import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/zone/safe_zone_response_model.dart';
import 'package:frontend_qlte/models/zone/create_safe_zone_request_model.dart';
import 'package:frontend_qlte/models/zone/update_safe_zone_request_model.dart';
import 'package:frontend_qlte/utils/api_helper.dart';

class SafeZoneService {
  static final String _baseUrl = '${Config_URL.baseUrl}SafeZone';

  /// 🔹 Lấy danh sách vùng an toàn theo user và child
  static Future<SafeZoneResponse> getSafeZoneByUserIdAndChildId(
    String userId,
    String childId,
  ) async {
    try {
      final uri = Uri.parse(
          '$_baseUrl/GetSafeZoneByUserIdAndChildId?userId=$userId&childId=$childId');

      final response = await ApiHelper.sendRequest("GET", uri);

      final jsonData = jsonDecode(response.body);
      final value = SafeZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return SafeZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return SafeZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Tạo vùng an toàn
  static Future<SafeZoneResponse> createSafeZone(
    CreateSafeZoneRequest request,
  ) async {
    try {
      final uri = Uri.parse('$_baseUrl/CreateSafeZone');

      final response = await ApiHelper.sendRequest(
        "POST",
        uri,
        body: jsonEncode(request.toJson()),
      );

      final jsonData = jsonDecode(response.body);
      final value = SafeZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return SafeZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return SafeZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Cập nhật vùng an toàn
  static Future<SafeZoneResponse> updateSafeZone(
    UpdateSafeZoneRequest request,
  ) async {
    try {
      final uri = Uri.parse('$_baseUrl/UpdateSafeZone');

      final response = await ApiHelper.sendRequest(
        "PUT",
        uri,
        body: jsonEncode(request.toJson()),
      );

      final jsonData = jsonDecode(response.body);
      final value = SafeZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return SafeZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return SafeZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Xóa vùng an toàn
  static Future<SafeZoneResponse> deleteSafeZone(String safeZoneId) async {
    try {
      final uri = Uri.parse('$_baseUrl/DeleteSafeZone?safeZoneId=$safeZoneId');

      final response = await ApiHelper.sendRequest("DELETE", uri);

      final jsonData = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return SafeZoneResponse.fromJson(jsonData);
      } else {
        return SafeZoneResponse(
          status: false,
          msg: jsonData['msg'] ?? 'Xóa vùng an toàn thất bại',
          data: [],
        );
      }
    } catch (e) {
      return SafeZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }
}
