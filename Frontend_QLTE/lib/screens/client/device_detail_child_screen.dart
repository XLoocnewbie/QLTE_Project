import 'dart:async';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/device_info_model.dart';
import 'package:frontend_qlte/services/device_info_service.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class DeviceDetailChildScreen extends StatefulWidget {
  final DeviceInfoModel device;

  const DeviceDetailChildScreen({
    super.key,
    required this.device,
  });

  @override
  State<DeviceDetailChildScreen> createState() => _DeviceDetailChildScreenState();
}

class _DeviceDetailChildScreenState extends State<DeviceDetailChildScreen> {
  bool _loading = false;
  bool _isTracking = false;
  bool _isLocked = false;
  Timer? _statusTimer;

  final SignalRService _signalRService = SignalRService();

  @override
  void initState() {
    super.initState();
    _isTracking = widget.device.isTracking;
    _isLocked = widget.device.isLocked;

    _initSignalR();

    if (_isTracking) _startAutoUpdateStatus();
  }

  /// 🛰️ Khởi tạo và lắng nghe SignalR
  Future<void> _initSignalR() async {
    try {
      await _signalRService.connectDeviceHub();

      // 🛰️ Nhận sự kiện thay đổi theo dõi
      _signalRService.onDeviceTrackingChanged((data) {
        final isTracking = data["isTracking"] ?? false;
        print("📡 [Realtime] Trạng thái theo dõi thay đổi: $isTracking");

        setState(() {
          _isTracking = isTracking;
        });

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(isTracking
                ? "🔵 Thiết bị đang được theo dõi"
                : "⚪ Thiết bị đã dừng theo dõi"),
            backgroundColor: isTracking ? Colors.green : Colors.grey,
          ),
        );

        if (isTracking) {
          _startAutoUpdateStatus();
        } else {
          _stopAutoUpdateStatus();
        }
      });

      // 🔒 Nhận sự kiện khoá thiết bị
      _signalRService.onDeviceLocked((data) {
        setState(() => _isLocked = true);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("🔒 Thiết bị đã bị khoá bởi phụ huynh")),
        );
      });

      // 🔓 Nhận sự kiện mở khoá thiết bị
      _signalRService.onDeviceUnlocked((data) {
        setState(() => _isLocked = false);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("🔓 Thiết bị đã được mở khoá")),
        );
      });
    } catch (e) {
      print("⚠️ Lỗi khi khởi tạo SignalR trong DeviceDetailChildScreen: $e");
    }
  }

  /// 🧩 Bật / Tắt tracking
  Future<void> _toggleTracking() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    if (token == null || widget.device.childId.isEmpty) return;

    setState(() => _loading = true);

    try {
      final result = await DeviceInfoService.setTracking(
        widget.device.childId,
        !_isTracking,
        token,
      );

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result['message']),
          backgroundColor:
          result['success'] ? Colors.teal : Colors.redAccent,
        ),
      );

      if (result['success']) {
        setState(() => _isTracking = !_isTracking);
        if (_isTracking) {
          _startAutoUpdateStatus();
        } else {
          _stopAutoUpdateStatus();
        }
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("⚠️ Lỗi bật/tắt theo dõi: $e")),
      );
    }

    setState(() => _loading = false);
  }

  /// 🧩 Khoá / Mở khoá thiết bị
  Future<void> _toggleLock() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    if (token == null || widget.device.childId.isEmpty) return;

    setState(() => _loading = true);

    try {
      final result = _isLocked
          ? await DeviceInfoService.unlockDevice(widget.device.childId, token)
          : await DeviceInfoService.lockDevice(widget.device.childId, token);

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result['message']),
          backgroundColor:
          result['success'] ? Colors.blue : Colors.redAccent,
        ),
      );

      if (result['success']) {
        setState(() => _isLocked = !_isLocked);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("⚠️ Lỗi thay đổi trạng thái thiết bị: $e")),
      );
    }

    setState(() => _loading = false);
  }

  /// 🕐 Tự động gửi trạng thái pin / online (mỗi 5 phút)
  void _startAutoUpdateStatus() {
    _stopAutoUpdateStatus(); // dừng nếu có timer cũ
    _statusTimer = Timer.periodic(const Duration(minutes: 5), (timer) async {
      if (!mounted) return;
      try {
        final randomPin = 60 + Random().nextInt(40); // 60–99%
        await DeviceInfoService.updateStatus(
          deviceId: widget.device.deviceId ,
          pin: randomPin,
          online: true,
        );
        print("📡 [Auto] Đã cập nhật trạng thái thiết bị ${widget.device.tenThietBi}");
      } catch (e) {
        print("⚠️ [Auto] Lỗi cập nhật: $e");
      }
    });
  }

  /// 🧩 Dừng auto update
  void _stopAutoUpdateStatus() {
    _statusTimer?.cancel();
    _statusTimer = null;
  }

  @override
  void dispose() {
    _stopAutoUpdateStatus();
    _signalRService.disconnectDeviceHub();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final device = widget.device;

    return Scaffold(
      appBar: AppBar(
        title: Text(device.tenThietBi ?? "Chi tiết thiết bị"),
        backgroundColor: Colors.teal,
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Icon(
                Icons.phone_android,
                size: 100,
                color: device.trangThaiOnline
                    ? Colors.green
                    : Colors.grey,
              ),
            ),
            const SizedBox(height: 20),
            Text("📱 Tên thiết bị: ${device.tenThietBi ?? '--'}",
                style: const TextStyle(fontSize: 18)),
            Text("IMEI: ${device.imei ?? '--'}"),
            Text("Pin: ${device.pin ?? '--'}%"),
            const Divider(height: 30),

            // 🛰️ Theo dõi định kỳ
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  "Theo dõi định kỳ (5 phút/lần)",
                  style: TextStyle(fontSize: 16),
                ),
                Switch(
                  value: _isTracking,
                  onChanged: (_) => _toggleTracking(),
                  activeColor: Colors.teal,
                ),
              ],
            ),

            const SizedBox(height: 20),

            // 🔒 Khoá / Mở khoá thiết bị
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  "Trạng thái thiết bị",
                  style: TextStyle(fontSize: 16),
                ),
                ElevatedButton.icon(
                  onPressed: _toggleLock,
                  icon: Icon(
                    _isLocked ? Icons.lock_open : Icons.lock_outline,
                    color: Colors.white,
                  ),
                  label: Text(
                    _isLocked ? "Mở khoá" : "Khoá thiết bị",
                    style: const TextStyle(color: Colors.white),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor:
                    _isLocked ? Colors.green : Colors.redAccent,
                    padding: const EdgeInsets.symmetric(
                        horizontal: 20, vertical: 10),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12)),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
