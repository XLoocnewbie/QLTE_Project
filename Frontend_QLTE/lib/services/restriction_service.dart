import 'package:flutter/services.dart';

class RestrictionService {
  static const platform = MethodChannel('com.frontend_qlte/restriction');

  static Future<void> updateBlockedApps(List<String> apps) async {
    try {
      final result = await platform.invokeMethod('updateBlockedApps', {'apps': apps});
      print("✅ $result");
    } catch (e) {
      print("❌ Lỗi khi gửi dữ liệu sang Android: $e");
    }
  }
}