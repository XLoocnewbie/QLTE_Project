import 'dart:async';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:geolocator/geolocator.dart';

class LocationAutoSender {
  final SignalRService _signalRService = SignalRService();
  StreamSubscription<Position>? _positionStream;
  bool _isRunning = false;

  /// Hàm khởi động gửi vị trí liên tục
  Future<void> start(String childId) async {
    if (_isRunning) return;
    _isRunning = true;

    // 1️ Xin quyền GPS
    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }

    if (permission == LocationPermission.deniedForever) {
      await Geolocator.openAppSettings();
      throw Exception(
        "Quyền truy cập vị trí bị từ chối vĩnh viễn. Vui lòng bật lại trong cài đặt ứng dụng.",
      );
    }

    if (permission == LocationPermission.denied) {
      throw Exception("Quyền truy cập vị trí bị từ chối.");
    }

    bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      await Geolocator.openLocationSettings();
      throw Exception("Vui lòng bật GPS để tiếp tục.");
    }

    // 3️ Kết nối tới SignalR
    await _signalRService.connect();

    // 4 Theo dõi vị trí và gửi liên tục
    _positionStream =
        Geolocator.getPositionStream(
          locationSettings: const LocationSettings(
            accuracy: LocationAccuracy.high,
            distanceFilter: 10, // chỉ gửi khi di chuyển trên 10m
          ),
        ).listen((position) async {
          await _signalRService.sendLocation(
            childId,
            position.latitude,
            position.longitude,
          );
          print(
            "📡 Vị trí cập nhật: ${position.latitude}, ${position.longitude}",
          );
        });
  }

  /// Dừng gửi vị trí
  void stop() {
    _positionStream?.cancel();
    _isRunning = false;
  }
}
