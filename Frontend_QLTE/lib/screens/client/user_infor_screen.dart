import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../config/config_url.dart';
import 'package:frontend_qlte/models/user_response_model.dart';
import '../../services/user_service.dart';

class UserInforScreen extends StatefulWidget {
  const UserInforScreen({super.key});

  @override
  State<UserInforScreen> createState() => _UserInforScreenState();
}

class _UserInforScreenState extends State<UserInforScreen> {
  final _formKey = GlobalKey<FormState>();

  Future<UserResponseModel?>? _futureUser;
  String? _token;
  String? _userId;
  String? _userName;

  final _nameController = TextEditingController();
  final _moTaController = TextEditingController();
  final _phoneController = TextEditingController();
  int? _gioiTinh;
  File? _selectedImage;

  @override
  void initState() {
    super.initState();
    _initAndLoadUser();
  }

  Future<void> _initAndLoadUser() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");
    _userId = prefs.getString("user_id");

    if (_token == null || _userId == null) {
      setState(() => _futureUser = Future.value(null));
      return;
    }

    setState(() {
      _futureUser = UserService.getUserByUserId(_userId!);
    });
  }

  Future<void> _pickImage() async {
    final picked = await ImagePicker().pickImage(source: ImageSource.gallery);
    if (picked != null) setState(() => _selectedImage = File(picked.path));
  }

  /// ✅ Fix lỗi ảnh — ghép domain nếu chỉ là đường dẫn tương đối
  String _buildAvatarUrl(String? avatarND) {
    if (avatarND == null || avatarND.isEmpty) {
      return "https://placehold.co/150x150?text=User";
    }

    // Nếu backend trả về full URL thì giữ nguyên
    if (avatarND.startsWith("http")) return avatarND;

    // Nếu trả về tương đối, ghép domain backend
    final base = Config_URL.baseUrl.replaceFirst("/api/", "");
    print("🖼️ Avatar URL được build: $base$avatarND");
    return "$base$avatarND".replaceAll("//uploads", "/uploads");
  }

  Future<void> _updateUser() async {
    if (_token == null || _userId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Vui lòng đăng nhập lại.")),
      );
      return;
    }

    if (_nameController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Tên người dùng không được để trống.")),
      );
      return;
    }

    final result = await UserService.updateInfoUser(
      userId: _userId!,
      userName: _userName ?? "",
      nameND: _nameController.text.trim(),
      moTa: _moTaController.text.trim(),
      gioiTinh: _gioiTinh,
      phoneNumber: _phoneController.text.trim(),
      avatarPath: _selectedImage?.path,
      token: _token!,
    );

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(result["message"] ?? "Cập nhật xong")),
    );

    if (result["success"] == true) {
      setState(() {
        _futureUser = UserService.getUserByUserId(_userId!);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Thông tin người dùng")),
      body: _futureUser == null
          ? const Center(child: CircularProgressIndicator())
          : FutureBuilder<UserResponseModel?>(
        future: _futureUser!,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (!snapshot.hasData) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text(
                    "Bạn chưa đăng nhập hoặc không tìm thấy người dùng.",
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 16),
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () {
                      Navigator.pushReplacementNamed(context, '/login');
                    },
                    child: const Text("Đăng nhập ngay"),
                  ),
                ],
              ),
            );
          }

          final user = snapshot.data!;
          _userName = user.userName; // ✅ Lưu username để gửi lại khi update
          _nameController.text = user.nameND ;
          _moTaController.text = user.mota ?? "";
          _phoneController.text = user.phoneNumber ;
          _gioiTinh ??= user.gioiTinh;

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Center(
                    child: Stack(
                      alignment: Alignment.bottomRight,
                      children: [
                        CircleAvatar(
                          radius: 60,
                          backgroundImage: _selectedImage != null
                              ? FileImage(_selectedImage!)
                              : (user.avatarND.isNotEmpty
                              ? NetworkImage(user.avatarND)
                              : const AssetImage(
                              "assets/avatar_placeholder.png"))
                          as ImageProvider
                        ),
                        IconButton(
                          icon: const Icon(Icons.camera_alt),
                          onPressed: _pickImage,
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),

                  TextFormField(
                    controller: _nameController,
                    decoration: const InputDecoration(
                      labelText: "Tên người dùng",
                      border: OutlineInputBorder(),
                    ),
                  ),
                  const SizedBox(height: 10),

                  DropdownButtonFormField<int>(
                    decoration: const InputDecoration(
                      labelText: "Giới tính",
                      border: OutlineInputBorder(),
                    ),
                    value: _gioiTinh,
                    items: const [
                      DropdownMenuItem(value: 0, child: Text("Nam")),
                      DropdownMenuItem(value: 1, child: Text("Nữ")),
                      DropdownMenuItem(value: 2, child: Text("Khác")),
                    ],
                    onChanged: (v) => setState(() => _gioiTinh = v),
                  ),
                  const SizedBox(height: 10),

                  TextFormField(
                    controller: _moTaController,
                    decoration: const InputDecoration(
                      labelText: "Mô tả",
                      border: OutlineInputBorder(),
                    ),
                    maxLines: 3,
                  ),
                  const SizedBox(height: 10),

                  TextFormField(
                    controller: _phoneController,
                    keyboardType: TextInputType.phone,
                    decoration: const InputDecoration(
                      labelText: "Số điện thoại",
                      border: OutlineInputBorder(),
                    ),
                  ),
                  const SizedBox(height: 20),

                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton.icon(
                      icon: const Icon(Icons.save),
                      label: const Text("Cập nhật"),
                      onPressed: _updateUser,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
