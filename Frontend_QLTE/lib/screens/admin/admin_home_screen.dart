import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:jwt_decoder/jwt_decoder.dart';

class AdminHomeScreen extends StatefulWidget {
  const AdminHomeScreen({super.key});

  @override
  State<AdminHomeScreen> createState() => _AdminHomeScreenState();
}

class _AdminHomeScreenState extends State<AdminHomeScreen> {
  String? _email;
  String? _role;
  bool _isAllowed = false;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _checkAdminRole();
  }

  Future<void> _checkAdminRole() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');

    if (token != null && token.isNotEmpty) {
      try {
        final decodedToken = JwtDecoder.decode(token);

        final role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        final email = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

        setState(() {
          _email = email;
          _role = role;
          _isAllowed = role == 'Admin';
          _isLoading = false;
        });
      } catch (e) {
        setState(() {
          _isLoading = false;
          _isAllowed = false;
        });
      }
    } else {
      setState(() {
        _isLoading = false;
        _isAllowed = false;
      });
    }
  }

  Future<void> _logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('jwt_token');
    if (!mounted) return;
    Navigator.pushReplacementNamed(context, '/login');
  }

  void _openFeature(String routeName) {
    // Placeholder cho các chức năng quản lý (sẽ cập nhật sau)
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text("⏳ Đang mở tính năng '$routeName'...")),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.blue.shade50,
      appBar: AppBar(
        automaticallyImplyLeading: false,
        title: const Text("🛠 Trang quản trị viên"),
        backgroundColor: Colors.blueAccent,
        actions: [
          IconButton(
            onPressed: _logout,
            tooltip: "Đăng xuất",
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : !_isAllowed
          ? const Center(
        child: Text(
          "🚫 Bạn không có quyền truy cập trang này!",
          style: TextStyle(fontSize: 20, color: Colors.red),
          textAlign: TextAlign.center,
        ),
      )
          : Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 👤 Thông tin admin
            Card(
              elevation: 3,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Row(
                  children: [
                    const CircleAvatar(
                      radius: 30,
                      backgroundColor: Colors.blueAccent,
                      child: Icon(Icons.admin_panel_settings, color: Colors.white, size: 30),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text("Email: $_email", style: const TextStyle(fontSize: 16)),
                          const SizedBox(height: 4),
                          Text("Vai trò: $_role", style: const TextStyle(fontSize: 16)),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 20),

            const Text(
              "Tính năng quản trị",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 10),

            // 🧩 Grid các chức năng quản lý
            Expanded(
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 16,
                mainAxisSpacing: 16,
                children: [
                  _buildFeatureCard(
                    icon: Icons.people,
                    title: "Quản lý người dùng",
                    color: Colors.deepPurpleAccent,
                    onTap: () => Navigator.pushNamed(context, '/admin_user'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.child_care_sharp,
                    title: "Quản lý trẻ em của người dùng",
                    color: Colors.deepOrangeAccent,
                    onTap: () {
                      final prefs = SharedPreferences.getInstance();
                      prefs.then((p) {
                        final token = p.getString('jwt_token');
                        if (token != null && token.isNotEmpty) {
                          Navigator.pushNamed(
                            context,
                            '/admin_children',
                            arguments: {'token': token},
                          );
                        } else {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(content: Text("Không tìm thấy token đăng nhập!")),
                          );
                        }
                      });
                    },
                  ),
                  _buildFeatureCard(
                    icon: Icons.info_outline,
                    title: "Quản lý refresh token",
                    color: Colors.redAccent,
                    onTap: () =>
                        Navigator.pushNamed(context, '/admin_refresh_token'),
                  ),
                  _buildFeatureCard(
                    icon: Icons.info_outline,
                    title: "Thông tin cá nhân",
                    color: Colors.lightBlueAccent,
                    onTap: () =>
                        Navigator.pushNamed(context, '/userinfo'),
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
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15)),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(15),
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
                const SizedBox(height: 10),
                Text(
                  title,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
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