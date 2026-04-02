import 'dart:convert';

import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/location_history_response_model.dart';
import 'package:frontend_qlte/utils/api_helper.dart';

class LocationService {
  static final String _baseUrl = '${Config_URL.baseUrl}Location';

  static Future<LocationHistoryResponseModel> getLocationHistoryNewByChildId(
    String childId,
  ) async {
    try {
      final uri = Uri.parse('$_baseUrl/GetLocationHistoryNew?childId=$childId');

      // 👇 Gọi API thông qua ApiHelper (đồng bộ hoàn toàn)
      final response = await ApiHelper.sendRequest('GET', uri);

      final jsonData = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return LocationHistoryResponseModel.fromJson(jsonData);
      } else {
        return LocationHistoryResponseModel(
          status: false,
          msg: jsonData['msg'] ?? 'Lỗi: ${response.statusCode}',
          data: [],
        );
      }
    } catch (e) {
      return LocationHistoryResponseModel(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }
}
