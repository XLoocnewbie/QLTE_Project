import 'dart:async';
import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:vibration/vibration.dart';
import 'package:audioplayers/audioplayers.dart';
import 'package:frontend_qlte/services/sos_sender.dart'; // ✅ Dùng SOSSender mới


class SOSRequestChildrenScreen extends StatefulWidget {
  const SOSRequestChildrenScreen({super.key});

  @override
  State<SOSRequestChildrenScreen> createState() => _SOSRequestChildrenScreenState();
}

class _SOSRequestChildrenScreenState extends State<SOSRequestChildrenScreen>
    with SingleTickerProviderStateMixin {
  bool _isSending = true;
  bool _isSuccess = false;
  late AnimationController _animationController;
  final SOSSender _sosSender = SOSSender(); // ✅ Dùng class mới

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800),
    )..repeat(reverse: true);

    _startSOSProcess();
  }

  /// 🚨 Hàm tổng hợp quy trình gửi SOS (API + Realtime)
  Future<void> _startSOSProcess() async {
    try {
      final AudioPlayer audioPlayer = AudioPlayer();

      // 🛰️ Kiểm tra GPS
      final serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        await Geolocator.openLocationSettings();
        throw Exception("GPS chưa bật, vui lòng bật vị trí để gửi SOS.");
      }

      // 🔐 Kiểm tra quyền truy cập vị trí
      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
        if (permission == LocationPermission.denied) {
          throw Exception("Ứng dụng không có quyền truy cập vị trí!");
        }
      }

      print("🚀 Bắt đầu gửi SOS qua SOSSender...");
      final result = await _sosSender.sendSOS();

      if (result["success"] == true) {
        // 🔊 Rung và phát âm thanh
        if (await Vibration.hasVibrator()) {
          Vibration.vibrate(duration: 1500);
        }
        await audioPlayer.play(AssetSource('sounds/sos_alarm.mp3'));

        setState(() {
          _isSuccess = true;
          _isSending = false;
        });

        print("✅ SOS đã gửi thành công và phát tín hiệu realtime!");
      } else {
        throw Exception(result["message"]);
      }
    } catch (e) {
      print("❌ Lỗi trong quá trình gửi SOS: $e");
      setState(() {
        _isSending = false;
        _isSuccess = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("❌ Gửi tín hiệu thất bại: $e"),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      body: Center(
        child: _isSending
            ? Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: const [
            CircularProgressIndicator(color: Colors.redAccent),
            SizedBox(height: 20),
            Text(
              "Đang gửi tín hiệu SOS...",
              style: TextStyle(color: Colors.white, fontSize: 18),
            ),
          ],
        )
            : _isSuccess
            ? AnimatedBuilder(
          animation: _animationController,
          builder: (context, child) {
            return Container(
              color: Color.lerp(
                Colors.black,
                Colors.redAccent.withOpacity(0.6),
                _animationController.value,
              ),
              child: Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Icon(
                      Icons.warning_amber_rounded,
                      size: 120,
                      color: Colors.white,
                    ),
                    const SizedBox(height: 20),
                    const Text(
                      "🚨 Tín hiệu SOS đã được gửi!",
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 12),
                    const Text(
                      "Phụ huynh và trung tâm đã nhận được tín hiệu của bạn.",
                      style: TextStyle(
                        color: Colors.white70,
                        fontSize: 16,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 40),
                    ElevatedButton.icon(
                      onPressed: () => Navigator.pop(context),
                      icon: const Icon(Icons.home),
                      label: const Text(
                        "Quay về màn hình chính",
                        style: TextStyle(fontSize: 16),
                      ),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.teal,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 24, vertical: 14),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        )
            : const Text(
          "Không thể gửi tín hiệu SOS.",
          style: TextStyle(color: Colors.white, fontSize: 18),
        ),
      ),
    );
  }
}
