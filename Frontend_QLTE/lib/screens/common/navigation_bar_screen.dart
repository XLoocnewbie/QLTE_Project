import 'package:flutter/material.dart';
import 'package:frontend_qlte/screens/client/child/children_list_screen.dart';
import 'package:frontend_qlte/screens/client/home_screen.dart';
import 'package:frontend_qlte/screens/client/message/message_screen.dart';
import 'package:frontend_qlte/screens/client/setting_screen.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:frontend_qlte/screens/client/sos_request_parent_screen.dart'; // ✅ Màn hình SOS
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

class NavigationBarScreen extends StatefulWidget {
  const NavigationBarScreen({super.key});

  @override
  State<NavigationBarScreen> createState() => _NavigationBarScreenState();
}

class _NavigationBarScreenState extends State<NavigationBarScreen> {
  int _selectedIndex = 0;
  late final List<Widget> _widgetOptions;
  final SignalRService _signalRService = SignalRService();
  final SignalRService _signalR = SignalRService();

  bool _isNavigating = false; // 🔒 Ngăn chặn push nhiều lần khi có nhiều SOS tới

  @override
  void initState() {
    super.initState();
    _initSignalR(); // ✅ Khởi động lắng nghe realtime SOS

    _widgetOptions = <Widget>[
      const HomeScreen(),
      const ChildrenListScreen(),
      ChangeNotifierProvider(
        create: (_) => MessageController(),
        child: const MessageScreen(),
      ),
      const Center(
        child: const SettingScreen(),
      ),
    ];
    _initSignalRGroup();
  }

  Future<void> _initSignalRGroup() async {
    try {
      // 🔹 Lấy token & userId
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";
      final userId = JwtHelper.getUserId(token);

      // 🔹 Kết nối SignalR
      await _signalRService.connect();

      // 🔹 Lấy danh sách con của phụ huynh
      final childrenRes = await ChildService.getChildrenByUserId(
        userId!,
      );
      if (childrenRes.status && childrenRes.data.isNotEmpty) {
        for (var child in childrenRes.data) {
          await _signalRService.joinChildGroup(child.childId);
        }
      }

      // 🔹 Lắng nghe cảnh báo khi trẻ ra khỏi vùng an toàn
      _signalRService.onChildLeftSafeZone((data) {
        final tenTre = data['tenTre'];
        final message = data['message'];
        _showWarningDialog("$tenTre cảnh báo!", message);
      });

      print("✅ Đã kết nối SignalR và join group các con");
    } catch (e) {
      print("⚠️ Lỗi khi join group SignalR: $e");
    }
  }

  void _showWarningDialog(String title, String content) {
    if (!mounted) return;
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title, style: const TextStyle(color: Colors.red)),
        content: Text(content),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Đóng"),
          ),
        ],
      ),
    );
  }

  /// ✅ Kết nối SOSHub và lắng nghe realtime từ child
  Future<void> _initSignalR() async {
    try {
      await _signalR.connectSOS();
      print("🛰️ Parent connected to SOSHub");

      // 🧩 Lấy childId để join vào group SOS riêng của từng con
      final prefs = await SharedPreferences.getInstance();
      final childId = prefs.getString('child_id');

      if (childId != null && childId.isNotEmpty) {
        await _signalR.joinSOSGroup(childId);
        print("👨‍👧 Parent joined SOS group for child-$childId");
      } else {
        print("⚠️ Không tìm thấy child_id trong SharedPreferences.");
      }

      // 🚨 Lắng nghe SOS realtime
      _signalR.onReceiveSOS((data) async {
        print("🚨 Parent nhận SOS realtime: $data");

        if (!mounted || _isNavigating) return; // Ngăn push trùng
        _isNavigating = true;

        // 🔔 Hiển thị cảnh báo nhanh
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text("🚨 Cảnh báo khẩn cấp! Con bạn vừa gửi tín hiệu SOS!"),
            backgroundColor: Colors.redAccent,
            duration: Duration(seconds: 3),
          ),
        );

        // 🕒 Chờ 1 chút để người dùng đọc cảnh báo
        await Future.delayed(const Duration(seconds: 1));

        // ⏩ Chuyển sang màn hình SOS chi tiết
        await Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => SOSRequestParentScreen(sosData: data),
          ),
        );

        _isNavigating = false; // 🔓 Cho phép điều hướng lại
      });

    } catch (e) {
      print("❌ Lỗi khi kết nối SOS realtime: $e");
    }
  }

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  void dispose() {
    _signalR.disconnectAll(); // 🔌 Ngắt toàn bộ kết nối SignalR khi thoát
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: _widgetOptions[_selectedIndex],
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.format_list_bulleted_outlined),
            label: 'Bản tin',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.my_location),
            label: 'Vị trí',
          ),
          BottomNavigationBarItem(icon: Icon(Icons.message), label: 'Message'),
          BottomNavigationBarItem(icon: Icon(Icons.settings), label: 'Cài đặt'),
        ],
        currentIndex: _selectedIndex,
        selectedItemColor: Colors.blue,
        unselectedItemColor: Colors.grey,
        onTap: _onItemTapped,
      ),
    );
  }
}
