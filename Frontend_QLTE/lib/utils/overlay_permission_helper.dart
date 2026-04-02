import 'package:flutter/services.dart';

class OverlayPermissionHelper {
  static const _channel = MethodChannel('com.frontend_qlte/restriction');

  static Future<void> requestOverlayPermission() async {
    try {
      await _channel.invokeMethod('openOverlaySettings');
    } catch (e) {
      print("⚠️ Không thể mở Overlay Settings: $e");
    }
  }
}
