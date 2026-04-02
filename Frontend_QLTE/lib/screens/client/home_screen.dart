import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:jwt_decoder/jwt_decoder.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  String? _userEmail;
  String? _role;
  bool _isExpired = false;
  String? _token;

  @override
  void initState() {
    super.initState();
    _loadTokenAndUser();
  }

  Future<void> _loadTokenAndUser() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');

    if (token != null) {
      final decodedToken = JwtDecoder.decode(token);
      final role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

      setState(() {
        _token = token;
        _userEmail = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ?? "Không rõ";
        _role = role ?? "Không rõ";
        _isExpired = JwtDecoder.isExpired(token);
      });

      // 🧩 Nếu không phải Parent thì điều hướng sang HomeChildren
      if (_role != "Parent") {
        if (!mounted) return;
        Navigator.pushReplacementNamed(context, '/home_children');
      }
    } else {
      if (!mounted) return;
      Navigator.pushReplacementNamed(context, '/login');
    }
  }

  void _logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('jwt_token');
    if (!mounted) return;
    Navigator.pushReplacementNamed(context, '/login');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.lightBlue.shade50,
      appBar: AppBar(
        automaticallyImplyLeading: false,
        title: const Text("👨‍👩‍👧 KidGuardian - Trang Phụ huynh"),
        backgroundColor: Colors.indigoAccent,
        actions: [
          IconButton(
            onPressed: _logout,
            tooltip: "Đăng xuất",
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: _userEmail == null
          ? const Center(child: CircularProgressIndicator())
          : Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 👤 Thông tin tài khoản
            Card(
              elevation: 4,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Row(
                  children: [
                    const CircleAvatar(
                      radius: 30,
                      backgroundColor: Colors.indigo,
                      child: Icon(Icons.person, color: Colors.white, size: 30),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text("📧 Email: $_userEmail", style: const TextStyle(fontSize: 16)),
                          const SizedBox(height: 6),
                          Text("🧩 Vai trò: $_role", style: const TextStyle(fontSize: 15, color: Colors.grey)),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 25),

            const Text(
              "📋 Bảng điều khiển cho Phụ huynh",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 15),

            // 🧩 Lưới chức năng chính
            Expanded(
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 16,
                mainAxisSpacing: 16,
                children: [
                  _buildFeatureCard(
                    icon: Icons.child_care,
                    title: "Quản lý thông tin trẻ em",
                    color: Colors.purpleAccent,
                    onTap: () => Navigator.pushNamed(context, '/child_manager'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.schedule_rounded,
                    title: "Quản lý thời khoá biểu",
                    color: Colors.teal,
                    onTap: () => Navigator.pushNamed(context, '/schedule_parent'),
                  ),

                  _buildFeatureCard(
                    icon: Icons.schedule,
                    title: "Quản lý thời gian học của trẻ em",
                    color: Colors.lightBlueAccent,
                    onTap: () => Navigator.pushNamed(context, '/study_period_parent'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.phone_android_rounded,
                    title: "Thông tin máy của trẻ em",
                    color: Colors.orangeAccent,
                    onTap: () => Navigator.pushNamed(context, '/device_info_child'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.security_rounded,
                    title: "Quản lý hạn chế cho thiết bị",
                    color: Colors.deepOrange,
                    onTap: () => Navigator.pushNamed(context, '/restriction_parent'),
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
        elevation: 6,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18)),
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
                Icon(icon, color: Colors.white, size: 45),
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
