import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/user_response_model.dart';
import 'package:frontend_qlte/screens/auth/login_screen.dart';
import 'package:frontend_qlte/screens/client/user/update_user_info_screen.dart';
import 'package:frontend_qlte/services/user_service.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';

/// ======================
/// 🔹 CONTROLLER PHẦN TRÊN
/// ======================
class SettingController {
  String? fullName;
  String? role;
  String? email;
  String? avatar;
  String? userId;
  String? userName;
  String? phoneNumber;
  String? gioiTinh;
  String? moTa;

  /// 🔹 Load dữ liệu user từ token
  Future<void> loadUserData() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');

    if (token == null || token.isEmpty) {
      fullName = "Người dùng";
      role = "User";
      email = "Không có email";
      phoneNumber = "Không có số điện thoại";
      avatar = null;
      gioiTinh = "Chưa có";
      moTa = "Chưa có";
      userName = "Chưa có";
      return;
    }

    try {
      userId = JwtHelper.getUserId(token);
      if (userId != null) {
        final userResponse = await UserService.getUserByUserId(userId!);

        if (userResponse != null) {
          fullName = userResponse.nameND.isEmpty ? "Không có tên" : userResponse.nameND;
          role = JwtHelper.getRole(token);
          email = userResponse.email.isEmpty ? "Không có email" : userResponse.email;
          avatar = userResponse.avatarND;
          phoneNumber = userResponse.phoneNumber;
          if (userResponse.gioiTinh == 1) {
            gioiTinh = "Nam";
          } else if (userResponse.gioiTinh == 2) {
            gioiTinh = "Nữ";
          } else {
            gioiTinh = "Chưa xác định"; // hoặc null nếu bạn muốn
          }
          moTa = userResponse.mota;
          userName = userResponse.userName;
          print("usp mota: ${userResponse.mota}");
        } else {
          fullName = "Không thể tải thông tin";
          role = "User";
          phoneNumber = "Không có số điện thoại";
          email = "";
        }
      }
    } catch (e) {
      print("Lỗi khi decode token hoặc tải user: $e");
      fullName = "Lỗi tải thông tin";
      phoneNumber = "Không có số điện thoại";
      role = "User";
      email = "";
    }
  }

  /// 🔹 Đăng xuất
  Future<void> logout(BuildContext context) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();

    if (context.mounted) {
      Navigator.pushAndRemoveUntil(
        context,
        MaterialPageRoute(builder: (_) => const LoginScreen()),
        (route) => false,
      );
    }
  }

  /// 🔹 Khi ấn “Cập nhật thông tin”
  Future<UserResponseModel?> onUpdatePressed(BuildContext context) async {
    final updatedUser = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => UpdateInfoUserScreen(
          fullName: fullName,
          userName: userName,
          avatar: avatar,
          userId: userId,
          phoneNumber: phoneNumber,
          gioiTinh: gioiTinh,
          moTa: moTa,
        ),
      ),
    );

    if (updatedUser is UserResponseModel) {
      fullName = updatedUser.nameND.isNotEmpty ? updatedUser.nameND : "Người Dùng"  ;
      email = updatedUser.email.isNotEmpty ? updatedUser.email : "Email không xác định";
      phoneNumber = updatedUser.phoneNumber;
      avatar = updatedUser.avatarND;
      gioiTinh = updatedUser.gioiTinh == 1
          ? "Nam"
          : updatedUser.gioiTinh == 2
          ? "Nữ"
          : "Chưa xác định";
      moTa = updatedUser.mota;
      userName = updatedUser.userName;
      return updatedUser;
    }

    return null;
  }
}

/// ======================
/// 🔹 GIAO DIỆN (UI) PHẦN DƯỚI
/// ======================
class SettingScreen extends StatefulWidget {
  const SettingScreen({super.key});

  @override
  State<SettingScreen> createState() => _SettingScreenState();
}

class _SettingScreenState extends State<SettingScreen> {
  final SettingController _controller = SettingController();

  @override
  void initState() {
    super.initState();
    _controller.loadUserData().then((_) => setState(() {}));
  }

  @override
  Widget build(BuildContext context) {
    final String? avatar = _controller.avatar;
    final isParent = _controller.role?.toLowerCase() == 'parent';
    final isAdmin = _controller.role?.toLowerCase() == 'admin';
    return Scaffold(
      backgroundColor: const Color(0xFFF9F9F9),
      appBar: AppBar(
        title: const Text('Cài đặt'),
        centerTitle: true,
        elevation: 0,
        automaticallyImplyLeading: false,
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const SizedBox(height: 40),

              // 🔹 Avatar
              CircleAvatar(
                radius: 45,
                backgroundColor: Colors.grey[300],
                backgroundImage: (avatar != null && avatar.isNotEmpty)
                    ? (avatar.startsWith("http")
                          ? NetworkImage(avatar)
                          : NetworkImage("${Config_URL.urlServer}$avatar"))
                    : const AssetImage('assets/images/default_avatar.png')
                          as ImageProvider,
              ),

              const SizedBox(height: 20),

              // 🔹 Họ tên
              Text(
                _controller.fullName ?? 'Người dùng',
                style: const TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                  color: Colors.teal,
                ),
              ),

              const SizedBox(height: 6),
              Text(
                _controller.email ?? 'Không có email',
                style: const TextStyle(fontSize: 15, color: Colors.grey),
              ),

              const SizedBox(height: 6),
              Text(
                'Vai trò: ${_controller.role ?? 'User'}',
                style: const TextStyle(
                  fontSize: 16,
                  color: Colors.blueGrey,
                  fontWeight: FontWeight.w500,
                ),
              ),

              const SizedBox(height: 40),
              if(isParent || isAdmin)
              // 🔹 Nút cập nhật thông tin
              SizedBox(
                width: double.infinity,
                child: ElevatedButton.icon(
                  icon: const Icon(Icons.edit),
                  label: const Text(
                    'Cập nhật thông tin',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blueAccent,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  onPressed: () async {
                    final updatedUser = await _controller.onUpdatePressed(
                      context,
                    );
                    if (updatedUser != null && mounted) {
                      setState(() {}); // Tự động render lại UI với dữ liệu mới
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(
                          content: Text('Cập nhật thông tin thành công!'),
                        ),
                      );
                    }
                  },
                ),
              ),

              const SizedBox(height: 16),

              // 🔹 Nút quản lý user (chỉ Admin)
              // if (isAdmin)
              //   SizedBox(
              //     width: double.infinity,
              //     child: ElevatedButton.icon(
              //       icon: const Icon(Icons.manage_accounts),
              //       label: const Text(
              //         'Quản lý User',
              //         style:
              //             TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              //       ),
              //       style: ElevatedButton.styleFrom(
              //         backgroundColor: Colors.green,
              //         foregroundColor: Colors.white,
              //         padding: const EdgeInsets.symmetric(vertical: 14),
              //         shape: RoundedRectangleBorder(
              //           borderRadius: BorderRadius.circular(12),
              //         ),
              //       ),
              //       onPressed: () {
              //         // TODO: mở màn hình quản lý user
              //       },
              //     ),
              //   ),
              const SizedBox(height: 16),
              // 🔹 Nút đăng xuất
              SizedBox(
                width: double.infinity,
                child: ElevatedButton.icon(
                  icon: const Icon(Icons.logout),
                  label: const Text(
                    'Đăng xuất',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.redAccent,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  onPressed: () => _controller.logout(context),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
