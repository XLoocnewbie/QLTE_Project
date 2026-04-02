import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:flutter/services.dart';
import 'package:frontend_qlte/utils/overlay_permission_helper.dart';

class HomeChildrenScreen extends StatefulWidget {
  const HomeChildrenScreen({super.key});

  @override
  State<HomeChildrenScreen> createState() => _HomeChildrenScreenState();
}

class _HomeChildrenScreenState extends State<HomeChildrenScreen> {
  String? _userEmail;
  String? _role;
  String? _token;
  bool _isExpired = false;

  // 🧩 MethodChannel kết nối Android
  static const platform = MethodChannel("com.frontend_qlte/restriction");

  @override
  void initState() {
    super.initState();
    _loadUser();
    _checkAccessibilityPermission(); // ✅ Kiểm tra quyền Trợ năng khi vào app
    _checkOverlayPermission(); // 🆕 Thêm kiểm tra quyền overlay
  }

  /// 🟣 Kiểm tra quyền Accessibility
  Future<void> _checkAccessibilityPermission() async {
    try {
      final bool isEnabled =
      await platform.invokeMethod("isAccessibilityEnabled");
      if (!isEnabled) {
        _showEnableAccessibilityDialog();
      } else {
        print("✅ Accessibility đã được bật.");
      }
    } catch (e) {
      print("❌ Lỗi kiểm tra quyền Accessibility: $e");
    }
  }

  Future<void> _checkOverlayPermission() async {
    await Future.delayed(const Duration(seconds: 1)); // đợi UI ổn định
    await OverlayPermissionHelper.requestOverlayPermission();
  }

  /// 🟢 Hiển thị popup yêu cầu bật quyền Trợ năng
  void _showEnableAccessibilityDialog() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => AlertDialog(
        title: const Text("Yêu cầu quyền quản lý thiết bị"),
        content: const Text(
          "Để bật tính năng học tập và chặn ứng dụng, vui lòng bật quyền Trợ năng (Accessibility) cho ứng dụng Frontend_QLTE.",
          style: TextStyle(fontSize: 15),
        ),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              platform.invokeMethod("openAccessibilitySettings");
            },
            child: const Text("Mở cài đặt"),
          ),
        ],
      ),
    );
  }

  /// 🧠 Load thông tin người dùng (giữ nguyên)
  Future<void> _loadUser() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    if (token != null) {
      final decodedToken = JwtDecoder.decode(token);
      setState(() {
        _token = token;
        _userEmail = decodedToken[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
        _role = decodedToken[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        _isExpired = JwtDecoder.isExpired(token);
      });
    }
  }

  /// 🆘 Dialog gửi SOS (giữ nguyên)
  void _showSOSConfirmDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("🚨 Xác nhận gửi tín hiệu SOS"),
        content: const Text(
          "Bạn có chắc muốn gửi tín hiệu ngay lập tức đến phụ huynh không?",
          style: TextStyle(fontSize: 16),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Huỷ"),
          ),
          ElevatedButton(
            style:
            ElevatedButton.styleFrom(backgroundColor: Colors.red),
            onPressed: () {
              Navigator.pop(context);
              Navigator.pushNamed(context, '/sos_request_children');
            },
            child: const Text("Xác nhận"),
          ),
        ],
      ),
    );
  }

  // 🧩 phần build giữ nguyên
  @override
  Widget build(BuildContext context) {
    return _userEmail == null
        ? const Center(child: CircularProgressIndicator())
        : Scaffold(
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: Colors.redAccent,
        icon: const Icon(Icons.warning_amber_rounded, size: 30),
        label: const Text(
          "Gửi yêu cầu SOS",
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        onPressed: _showSOSConfirmDialog,
      ),
      floatingActionButtonLocation:
      FloatingActionButtonLocation.centerFloat,
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 👦 Thông tin trẻ
            Card(
              elevation: 3,
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    const CircleAvatar(
                      radius: 30,
                      backgroundColor: Colors.teal,
                      child: Icon(Icons.child_care,
                          color: Colors.white, size: 30),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text("Email: $_userEmail",
                              style: const TextStyle(fontSize: 16)),
                          const SizedBox(height: 4),
                          Text("Role: $_role",
                              style: const TextStyle(
                                  color: Colors.grey)),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 20),
            const Text("🎯 Hoạt động hôm nay",
                style: TextStyle(
                    fontSize: 20, fontWeight: FontWeight.bold)),
            const SizedBox(height: 15),
            // 🧩 Các tính năng
            Expanded(
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 16,
                mainAxisSpacing: 16,
                children: [
                  _buildFeatureCard(
                    icon: Icons.event_note,
                    title: "Lịch thời khoá biểu",
                    color: Colors.purpleAccent,
                    onTap: () => Navigator.pushNamed(
                        context, '/schedule_children'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.event_note,
                    title: "Lịch kiểm tra",
                    color: Colors.orangeAccent,
                    onTap: () => Navigator.pushNamed(
                        context, '/exam_schedule_children'),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFeatureCard({
    required IconData icon,
    required String title,
    required Color color,
    required VoidCallback onTap,
  }) {
    return GestureDetector(
      onTap: onTap,
      child: Card(
        elevation: 5,
        shape:
        RoundedRectangleBorder(borderRadius: BorderRadius.circular(18)),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(18),
            gradient: LinearGradient(
              colors: [color.withOpacity(0.9), color.withOpacity(0.6)],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
          ),
          child: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(icon, color: Colors.white, size: 40),
                const SizedBox(height: 12),
                Text(
                  title,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    height: 1.3,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

