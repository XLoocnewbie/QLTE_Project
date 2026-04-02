import 'dart:async';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/screens/client/message/message_screen.dart';
import 'package:frontend_qlte/screens/client/child/home_children_screen.dart';
import 'package:frontend_qlte/screens/client/setting_screen.dart';
import 'package:frontend_qlte/services/auth.dart';
import 'package:frontend_qlte/services/location_auto_sender.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:frontend_qlte/services/device_info_service.dart';
import 'package:frontend_qlte/services/study_period_service.dart'; // 🟢 thêm dòng này
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:frontend_qlte/models/study_period_model.dart'; // 🟢 dùng để parse model
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:vibration/vibration.dart';
import 'package:flutter/services.dart';

class NavigationBarChildScreen extends StatefulWidget {
  const NavigationBarChildScreen({super.key});

  @override
  State<NavigationBarChildScreen> createState() =>
      _NavigationBarChildScreenState();
  static const platform = MethodChannel("com.frontend_qlte/restriction");
}

class _NavigationBarChildScreenState extends State<NavigationBarChildScreen>
    with SingleTickerProviderStateMixin {
  static const platform = MethodChannel("com.frontend_qlte/restriction");
  int _selectedIndex = 0;
  late final List<Widget> _widgetOptions;
  final LocationAutoSender _locationAutoSender = LocationAutoSender();
  final SignalRService _signalRService = SignalRService();

  bool _isLocked = false; // 🔒 từ DeviceHub
  bool _isInStudy = false; // 📚 từ StudyHub hoặc API kiểm tra
  String? _studyMessage;

  DateTime? _studyEndTime;
  Timer? _countdownTimer;
  Duration _remaining = Duration.zero;
  Timer? _statusTimer;

  late AnimationController _animController;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _startAutoLocation();
    _initSignalR();
    _startAutoUpdateStatus();
    _checkCurrentStudyPeriod(); // 🟢 kiểm tra khung giờ học khi mở app

    _animController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800),
    )..repeat(reverse: true);

    _scaleAnimation = Tween<double>(begin: 1.0, end: 1.15)
        .animate(CurvedAnimation(parent: _animController, curve: Curves.easeInOut));

    _widgetOptions = <Widget>[
      const HomeChildrenScreen(),
      const Center(child: Text('Trang Vị trí (Tự động gửi)', style: TextStyle(fontSize: 20))),
      ChangeNotifierProvider(
        create: (_) => MessageController(),
        child: const MessageScreen(),
      ),
      const Center(
        child: const SettingScreen(),
      ),
    ];
  }

  // 🟢 Kiểm tra khung giờ học hiện tại khi child vào app
  Future<void> _checkCurrentStudyPeriod() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final childId = prefs.getString("child_id");
      if (childId == null) return;

      final result = await StudyPeriodService.getActiveByChild(childId);
      if (result["success"] == true && result["data"] != null) {
        final StudyPeriod study = result["data"];
        print("📚 Child đang trong giờ học từ ${study.startTime} đến ${study.endTime}");

        setState(() {
          _isInStudy = true;
          _studyMessage = study.moTa ?? "Đang trong giờ học, vui lòng tập trung học tập.";
          // tạo endTime tương đối từ giờ hiện tại nếu backend trả về TimeSpan
          final now = DateTime.now();
          final timeParts = study.endTime.split(':');
          final h = int.tryParse(timeParts[0]) ?? 0;
          final m = int.tryParse(timeParts[1]) ?? 0;
          _studyEndTime = DateTime(now.year, now.month, now.day, h, m);
        });

        _showStudyPopup(_studyMessage ?? "Đang trong giờ học");
        _startCountdown();
      } else {
        print("✅ Không có khung giờ học đang bật cho child.");
      }
    } catch (e) {
      print("⚠️ Lỗi kiểm tra khung giờ học hiện tại: $e");
    }
  }

  // 🛰️ Khởi tạo SignalR (DeviceHub + StudyHub)
  Future<void> _initSignalR() async {
    try {
      await _signalRService.connectDeviceHub();
      await _signalRService.connectStudyHub(); // 📚 Kết nối thêm StudyHub

      // 🔒 DeviceHub: khoá thiết bị
      _signalRService.onDeviceLocked((data) async {
        setState(() => _isLocked = true);

        // rung nhẹ khi bị khoá
        if (await Vibration.hasVibrator()) {
          Vibration.vibrate(pattern: [0, 400, 150, 400]);
        }
      });

      // 🔓 DeviceHub: mở khoá
      _signalRService.onDeviceUnlocked((data) {
        setState(() => _isLocked = false);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text("✅ Thiết bị đã được mở khóa."),
            backgroundColor: Colors.green,
          ),
        );
      });

      // 📚 StudyHub: bật / tắt giờ học realtime
      _signalRService.onStudyPeriodChanged((data) async {
        final active = data["isActive"] ?? false;
        final message = data["message"] ?? "Cập nhật trạng thái giờ học";

        if (active) {
          // 📱 Danh sách app cần chặn trong giờ học
          final blockedApps = ["chrome", "youtube", "tiktok"];

          try {
            await platform.invokeMethod("updateBlockedApps", {"apps": blockedApps});
            print("🔒 Đã gửi danh sách chặn cho native: $blockedApps");
          } catch (e) {
            print("❌ Gửi danh sách chặn thất bại: $e");
          }

          // phần còn lại giữ nguyên
          _showStudyPopup(message);
          setState(() => _isInStudy = true);
        } else {
          // 🔓 Khi tắt giờ học → clear danh sách
          try {
            await platform.invokeMethod("updateBlockedApps", {"apps": []});
            print("🔓 Đã xoá danh sách chặn");
          } catch (e) {
            print("❌ Lỗi khi xoá danh sách chặn: $e");
          }

          setState(() => _isInStudy = false);
        }
      });
    } catch (e) {
      print("⚠️ Lỗi kết nối SignalR (DeviceHub/StudyHub): $e");
    }
  }

  // 🔢 Bắt đầu đếm ngược
  void _startCountdown() {
    _countdownTimer?.cancel();
    if (_studyEndTime == null) return;

    _countdownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
      final now = DateTime.now();
      final diff = _studyEndTime!.difference(now);
      if (diff.isNegative) {
        _stopCountdown();
        setState(() {
          _isInStudy = false;
          _remaining = Duration.zero;
        });
      } else {
        setState(() => _remaining = diff);
      }
    });
  }

  void _stopCountdown() {
    _countdownTimer?.cancel();
    _countdownTimer = null;
  }

  Future<void> _startAutoLocation() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString("jwt_token") ?? "";
    final userId = JwtHelper.getUserId(token);
    try {
      await _locationAutoSender.start(userId!);
      print("🚀 Bắt đầu chia sẻ vị trí tự động");
    } catch (e) {
      print("⚠️ Lỗi khởi động chia sẻ vị trí: $e");
    }
  }

  void _startAutoUpdateStatus() {
    _stopAutoUpdateStatus();
    _updateStatusNow();
    _statusTimer = Timer.periodic(const Duration(minutes: 5), (_) => _updateStatusNow());
  }

  Future<void> _updateStatusNow() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final deviceId = prefs.getString("device_id");
      if (deviceId == null) return;
      final randomPin = 60 + Random().nextInt(40);
      await DeviceInfoService.updateStatus(deviceId: deviceId, pin: randomPin, online: true);
      print("📡 [Auto] Gửi trạng thái pin=$randomPin% cho deviceId=$deviceId");
    } catch (e) {
      print("⚠️ [Auto] Lỗi khi gửi trạng thái thiết bị: $e");
    }
  }

  void _stopAutoUpdateStatus() {
    _statusTimer?.cancel();
    _statusTimer = null;
  }

  @override
  void dispose() {
    _locationAutoSender.stop();
    _stopAutoUpdateStatus();
    _stopCountdown();
    _animController.dispose();
    super.dispose();
  }

  void _onItemTapped(int index) {
    if (_isLocked || _isInStudy) return;
    setState(() => _selectedIndex = index);
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        Scaffold(
          appBar: AppBar(
            automaticallyImplyLeading: false,
            title: const Text('🧒 KidGuardian - Trẻ em'),
            backgroundColor: Colors.teal,
            actions: [
              IconButton(
                tooltip: 'Đăng xuất',
                icon: const Icon(Icons.logout),
                onPressed: () async {
                  final prefs = await SharedPreferences.getInstance();
                  
                  final refreshToken = prefs.getString("refresh_token")!;

                  final logout = await Auth.logout(refreshToken);
                  if(logout){
                    if (!context.mounted) return;
                    Navigator.pushReplacementNamed(context, '/login');

                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text("Đã đăng xuất thành công."),
                        duration: Duration(seconds: 2),
                      ),
                    );
                  }
                  ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text("Đã đăng xuất thất bại."),
                        duration: Duration(seconds: 2),
                      ),
                    );
                },
              ),
            ],
          ),
          body: _widgetOptions[_selectedIndex],
          bottomNavigationBar: BottomNavigationBar(
            type: BottomNavigationBarType.fixed,
            currentIndex: _selectedIndex,
            selectedItemColor: Colors.teal,
            unselectedItemColor: Colors.grey,
            onTap: _onItemTapped,
            items: const [
              BottomNavigationBarItem(icon: Icon(Icons.home_outlined), label: 'Trang chủ'),
              BottomNavigationBarItem(icon: Icon(Icons.my_location), label: 'Vị trí'),
              BottomNavigationBarItem(icon: Icon(Icons.message_outlined), label: 'Tin nhắn'),
              BottomNavigationBarItem(icon: Icon(Icons.settings), label: 'Cài đặt'),
            ],
          ),
        ),

        if (_isLocked)
          _buildOverlay(
            icon: Icons.lock_rounded,
            title: "Thiết bị của bạn đã bị khóa",
            message: "Phụ huynh đã tạm thời khóa thiết bị này.\nVui lòng chờ mở khóa để tiếp tục sử dụng.",
            color: Colors.redAccent,
          ),

        if (_isInStudy)
          _buildOverlay(
            icon: Icons.school_rounded,
            title: "⏰ Đang trong giờ học",
            message: _studyMessage ?? "Vui lòng tập trung học tập",
            color: Colors.orangeAccent,
            countdown: _remaining,
          ),
      ],
    );
  }

  void _showStudyPopup(String message) {
    if (!mounted) return;
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("⏰ Thông báo giờ học"),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Đã hiểu"),
          ),
        ],
      ),
    );
  }

  Widget _buildOverlay({
    required IconData icon,
    required String title,
    required String message,
    required Color color,
    Duration? countdown,
  }) {
    String timeLeft = '';
    if (countdown != null && countdown.inSeconds > 0) {
      final m = countdown.inMinutes;
      final s = countdown.inSeconds % 60;
      timeLeft = '⏳ Còn ${m.toString().padLeft(2, '0')}:${s.toString().padLeft(2, '0')}';
    }

    return Positioned.fill(
      child: Container(
        color: Colors.black.withOpacity(0.85),
        child: SafeArea(
          child: Center(
            child: ScaleTransition(
              scale: _scaleAnimation,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(icon, size: 120, color: color),
                  const SizedBox(height: 30),
                  Text(title,
                      style: const TextStyle(
                          color: Colors.white,
                          fontSize: 22,
                          fontWeight: FontWeight.bold),
                      textAlign: TextAlign.center),
                  const SizedBox(height: 16),
                  Text(message,
                      style: const TextStyle(color: Colors.white70, fontSize: 16),
                      textAlign: TextAlign.center),
                  if (timeLeft.isNotEmpty) ...[
                    const SizedBox(height: 20),
                    Text(timeLeft,
                        style: const TextStyle(
                            color: Colors.white,
                            fontSize: 20,
                            fontWeight: FontWeight.bold)),
                  ],
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
