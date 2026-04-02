import 'dart:convert';

import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/zone/danger_zone_response_model.dart';
import 'package:frontend_qlte/models/zone/create_danger_zone_request_model.dart';
import 'package:frontend_qlte/models/zone/update_danger_zone_request_model.dart';
import 'package:frontend_qlte/utils/api_helper.dart';

class DangerZoneService{
  static final String _baseUrl = '${Config_URL.baseUrl}DangerZone';

  static Future<DangerZoneResponse> getDangerZoneByUserIdAndChildId(
    String userId,
    String childId,
  ) async {
    try {
      final uri = Uri.parse(
          '$_baseUrl/GetDangerZoneByUserIdAndChildId?userId=$userId&childId=$childId');

      final response = await ApiHelper.sendRequest("GET", uri);

      final jsonData = jsonDecode(response.body);
      final value = DangerZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return DangerZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return DangerZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Tạo vùng nguy hiểm
  static Future<DangerZoneResponse> createDangerZone(
    CreateDangerZoneRequest request,
  ) async {
    try {
      final uri = Uri.parse('$_baseUrl/CreateDangerZone');

      final response = await ApiHelper.sendRequest(
        "POST",
        uri,
        body: jsonEncode(request.toJson()),
      );

      final jsonData = jsonDecode(response.body);
      final value = DangerZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return DangerZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return DangerZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Cập nhật vùng nguy hiểm
  static Future<DangerZoneResponse> updateDangerZone(
    UpdateDangerZoneRequest request,
  ) async {
    try {
      final uri = Uri.parse('$_baseUrl/UpdateDangerZone');

      final response = await ApiHelper.sendRequest(
        "PUT",
        uri,
        body: jsonEncode(request.toJson()),
      );

      final jsonData = jsonDecode(response.body);
      final value = DangerZoneResponse.fromJson(jsonData);

      if (response.statusCode == 200) {
        return value;
      } else {
        return DangerZoneResponse(
          status: false,
          msg: 'Lỗi: ${value.msg}',
          data: [],
        );
      }
    } catch (e) {
      return DangerZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Xóa vùng nguy hiểm
  static Future<DangerZoneResponse> deleteDangerZone(String dangerZoneId) async {
    try {
      final uri = Uri.parse('$_baseUrl/DeleteDangerZone?dangerZoneId=$dangerZoneId');

      final response = await ApiHelper.sendRequest("DELETE", uri);

      final jsonData = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return DangerZoneResponse.fromJson(jsonData);
      } else {
        return DangerZoneResponse(
          status: false,
          msg: jsonData['msg'] ?? 'Xóa vùng nguy hiểm thất bại',
          data: [],
        );
      }
    } catch (e) {
      return DangerZoneResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }
}