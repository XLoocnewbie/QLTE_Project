import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:image_picker/image_picker.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../models/user_response_model.dart';
import '../../services/user_service.dart';

class AdminUserScreen extends StatefulWidget {
  const AdminUserScreen({super.key});

  @override
  State<AdminUserScreen> createState() => _AdminUserScreenState();
}

class _AdminUserScreenState extends State<AdminUserScreen> {
  String? _token;
  bool _isLoading = false;
  List<UserResponseModel> _users = [];

  @override
  void initState() {
    super.initState();
    _loadUsers();
  }

  /// 🟢 Lấy danh sách Parent (lọc role)
  Future<void> _loadUsers() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");
    if (_token == null) return;

    setState(() => _isLoading = true);

    final users = await UserService.getAllUsers(token: _token!);

    // 🔹 Lọc chỉ những user có role là User hoặc Parent
    setState(() {
      _users = users
          .where((u) =>
      u.role.toLowerCase() == "user" || u.role.toLowerCase() == "parent")
          .toList();
      _isLoading = false;
    });
  }

  /// 🧩 Xây URL ảnh đại diện từ .env
  String _buildAvatarUrl(String? avatar) {
    final baseUrl = dotenv.env['URL_SERVER'] ?? '';
    if (avatar == null || avatar.isEmpty) {
      return "${baseUrl}assets/default_avatar.png";
    }
    if (avatar.startsWith("http")) return avatar;
    return "$baseUrl${avatar.startsWith('/') ? avatar.substring(1) : avatar}";
  }

  /// 👁️ Hiển thị chi tiết User
  void _showUserDetail(UserResponseModel user) {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("👤 Thông tin người dùng"),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            CircleAvatar(
              radius: 40,
              backgroundImage: (user.avatarND.isNotEmpty)
                  ? NetworkImage(_buildAvatarUrl(user.avatarND))
                  : const AssetImage("assets/default_avatar.png")
              as ImageProvider,
            ),
            const SizedBox(height: 10),
            Text("🆔 ID: ${user.userId}"),
            Text("👤 Họ tên: ${user.nameND}"),
            Text("📧 Email: ${user.email}"),
            Text("📱 SĐT: ${user.phoneNumber}"),
            Text("🚻 Giới tính: ${_getGenderName(user.gioiTinh)}"),
            Text("🎭 Quyền: ${user.role}"),
            Text("🔑 Loại đăng nhập: ${user.typeLogin}"),
            Text("🕓 Tạo: ${_formatDate(user.thoiGianTao)}"),
            Text("🕓 Cập nhật: ${_formatDate(user.thoiGianCapNhat)}"),
            if (user.mota != null && user.mota!.isNotEmpty)
              Text("💬 Mô tả: ${user.mota}"),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Đóng"),
          ),
        ],
      ),
    );
  }

  String _getGenderName(int? value) {
    switch (value) {
      case 1:
        return "Nam";
      case 0:
        return "Nữ";
      default:
        return "Khác";
    }
  }

  String _formatDate(DateTime date) {
    return "${date.day}/${date.month}/${date.year} ${date.hour}:${date.minute}";
  }

  /// ✏️ Cập nhật người dùng
  Future<void> _showEditDialog(UserResponseModel user) async {
    final nameCtrl = TextEditingController(text: user.nameND);
    final phoneCtrl = TextEditingController(text: user.phoneNumber);
    final motaCtrl = TextEditingController(text: user.mota ?? "");
    int gioiTinh = user.gioiTinh;
    File? selectedAvatar;

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) => AlertDialog(
          title: const Text("✏️ Cập nhật người dùng"),
          content: SingleChildScrollView(
            child: Column(
              children: [
                GestureDetector(
                  onTap: () async {
                    final picked =
                    await ImagePicker().pickImage(source: ImageSource.gallery);
                    if (picked != null) {
                      setStateDialog(() => selectedAvatar = File(picked.path));
                    }
                  },
                  child: CircleAvatar(
                    radius: 45,
                    backgroundImage: selectedAvatar != null
                        ? FileImage(selectedAvatar!)
                        : (user.avatarND.isNotEmpty)
                        ? NetworkImage(_buildAvatarUrl(user.avatarND))
                        : const AssetImage("assets/default_avatar.png")
                    as ImageProvider,
                    child: const Align(
                      alignment: Alignment.bottomRight,
                      child:
                      Icon(Icons.camera_alt, size: 22, color: Colors.white),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: nameCtrl,
                  decoration: const InputDecoration(labelText: "Họ tên"),
                ),
                TextField(
                  controller: phoneCtrl,
                  decoration: const InputDecoration(labelText: "Số điện thoại"),
                  keyboardType: TextInputType.phone,
                ),
                DropdownButtonFormField<int>(
                  value: gioiTinh,
                  decoration: const InputDecoration(labelText: "Giới tính"),
                  items: const [
                    DropdownMenuItem(value: 1, child: Text("Nam")),
                    DropdownMenuItem(value: 0, child: Text("Nữ")),
                    DropdownMenuItem(value: 2, child: Text("Khác")),
                  ],
                  onChanged: (v) => setStateDialog(() => gioiTinh = v ?? 2),
                ),
                TextField(
                  controller: motaCtrl,
                  decoration: const InputDecoration(labelText: "Mô tả"),
                  maxLines: 2,
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
                onPressed: () => Navigator.pop(context),
                child: const Text("Hủy")),
            ElevatedButton(
              onPressed: () async {
                if (_token == null) return;

                final result = await UserService.updateInfoUser(
                  token: _token!,
                  userId: user.userId,
                  userName: user.userName, // 🟢 backend bắt buộc
                  nameND: nameCtrl.text.trim(),
                  phoneNumber: phoneCtrl.text.trim(),
                  gioiTinh: gioiTinh,
                  moTa: motaCtrl.text.trim(),
                  avatarPath: selectedAvatar?.path,
                );

                if (mounted) {
                  Navigator.pop(context);
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                        content: Text(
                            result["message"] ?? "Cập nhật thành công")),
                  );
                  _loadUsers();
                }
              },
              child: const Text("Lưu"),
            ),
          ],
        ),
      ),
    );
  }

  /// 🗑️ Xóa người dùng
  Future<void> _deleteUser(UserResponseModel user) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xóa người dùng"),
        content: Text("Bạn có chắc muốn xóa ${user.nameND}?"),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(context, false),
              child: const Text("Hủy")),
          ElevatedButton(
              onPressed: () => Navigator.pop(context, true),
              child: const Text("Xóa")),
        ],
      ),
    );

    if (confirm == true && _token != null) {
      final result =
      await UserService.deleteUser(userId: user.userId, token: _token!);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result["message"] ?? "Đã xóa")),
      );
      _loadUsers();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("👥 Quản lý phụ huynh (Parent)"),
        backgroundColor: Colors.indigo,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadUsers),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _users.isEmpty
          ? const Center(child: Text("Chưa có người dùng nào."))
          : RefreshIndicator(
        onRefresh: _loadUsers,
        child: ListView.builder(
          padding: const EdgeInsets.all(12),
          itemCount: _users.length,
          itemBuilder: (context, index) {
            final u = _users[index];
            return Card(
              elevation: 3,
              margin:
              const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
              child: ListTile(
                leading: CircleAvatar(
                  radius: 25,
                  backgroundImage: (u.avatarND.isNotEmpty)
                      ? NetworkImage(_buildAvatarUrl(u.avatarND))
                      : const AssetImage("assets/default_avatar.png")
                  as ImageProvider,
                ),
                title: Text(u.nameND,
                    style: const TextStyle(
                        fontWeight: FontWeight.bold)),
                subtitle: Text(
                    "📧 ${u.email}\n📱 ${u.phoneNumber}\n🎭 ${u.role}"),
                trailing: Wrap(
                  spacing: 6,
                  children: [
                    IconButton(
                      icon: const Icon(Icons.visibility,
                          color: Colors.blue),
                      onPressed: () => _showUserDetail(u),
                    ),
                    IconButton(
                      icon: const Icon(Icons.edit,
                          color: Colors.orange),
                      onPressed: () => _showEditDialog(u),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete,
                          color: Colors.red),
                      onPressed: () => _deleteUser(u),
                    ),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}
