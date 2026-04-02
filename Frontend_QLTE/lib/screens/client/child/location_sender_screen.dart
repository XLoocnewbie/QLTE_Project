import 'dart:async';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:geolocator/geolocator.dart';

class LocationSenderScreen extends StatefulWidget {
  const LocationSenderScreen({super.key});

  @override
  State<LocationSenderScreen> createState() => _LocationSenderScreenState();
}

class _LocationSenderScreenState extends State<LocationSenderScreen> {
  final SignalRService _signalRService = SignalRService();
  Position? _currentPosition;
  Timer? _timer;
  bool _isConnected = false;

  @override
  void initState() {
    super.initState();
    _initSignalR();
  }

  Future<void> _initSignalR() async {
    await _signalRService.connect();
    setState(() => _isConnected = true);
    _startLocationUpdates();
  }

  void _startLocationUpdates() {
    _timer = Timer.periodic(const Duration(seconds: 10), (_) async {
      try {
        final position = await Geolocator.getCurrentPosition(
          desiredAccuracy: LocationAccuracy.high,
        );
        setState(() => _currentPosition = position);
        await _signalRService.sendLocation("child_001", position.latitude, position.longitude);
      } catch (e) {
        print("⚠️ Lỗi lấy vị trí: $e");
      }
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Chia sẻ vị trí")),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.my_location, size: 80, color: Colors.blueAccent),
            const SizedBox(height: 20),
            Text(
              _isConnected ? "✅ Đã kết nối SignalR" : "🔴 Đang kết nối...",
              style: TextStyle(
                color: _isConnected ? Colors.green : Colors.red,
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 30),
            _currentPosition == null
                ? const Text("Đang lấy vị trí GPS...",
                    style: TextStyle(fontSize: 16))
                : Column(
                    children: [
                      Text("Kinh độ: ${_currentPosition!.latitude}",
                          style: const TextStyle(fontSize: 16)),
                      Text("Vĩ độ: ${_currentPosition!.longitude}",
                          style: const TextStyle(fontSize: 16)),
                      const SizedBox(height: 20),
                      const Text("Vị trí đang được gửi cho Parent",
                          style: TextStyle(color: Colors.grey)),
                    ],
                  ),
          ],
        ),
      ),
    );
  }
}
