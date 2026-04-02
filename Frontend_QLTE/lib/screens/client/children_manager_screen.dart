import 'dart:io';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/screens/client/message/message_detail_screen.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../config/config_url.dart';
import '../../models/child_response_model.dart';
import '../../services/child_service.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

class ChildrenManagerScreen extends StatefulWidget {
  const ChildrenManagerScreen({super.key});

  @override
  State<ChildrenManagerScreen> createState() => _ChildrenManagerScreenState();
}

class _ChildrenManagerScreenState extends State<ChildrenManagerScreen> {
  String? _token;
  String? _userId;
  late Future<ChildrenResponse> _futureChildren;

  @override
  void initState() {
    super.initState();
    _loadChildren();
  }

  Future<void> _loadChildren() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");
    _userId = JwtHelper.getUserId(_token!);

    if (_token == null || _userId == null) return;

    setState(() {
      _futureChildren = ChildService.getChildrenByUserId(_userId!);
    });
  }

  /// Build avatar URL (xử lý tương đối)
  String _buildAvatarUrl(String? avatarND) {
    if (avatarND == null || avatarND.isEmpty) {
      return "https://placehold.co/100x100?text=Child";
    }
    if (avatarND.startsWith("http")) return avatarND;
    final base = Config_URL.baseUrl.replaceFirst("/api/", "");
    return "$base$avatarND".replaceAll("//uploads", "/uploads");
  }

  /// Thêm hoặc cập nhật trẻ
  Future<void> _showChildDialog({Child? child}) async {
    final nameCtrl = TextEditingController(text: child?.hoTen ?? "");
    final emailCtrl = TextEditingController();
    final phoneCtrl = TextEditingController(text: child?.phoneNumber ?? "");
    final passCtrl = TextEditingController();
    final confirmCtrl = TextEditingController();
    DateTime? birthDate = child?.ngaySinh;
    int gioiTinh = child?.gioiTinh == "Nam"
        ? 1
        : child?.gioiTinh == "Nữ"
        ? 0
        : 2;
    File? selectedAvatar;

    await showDialog(
      context: context,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          return AlertDialog(
            title: Text(
              child == null ? "➕ Thêm trẻ mới" : "✏️ Cập nhật thông tin",
            ),
            content: SingleChildScrollView(
              child: Column(
                children: [
                  // Avatar chọn ảnh
                  GestureDetector(
                    onTap: () async {
                      final picked = await ImagePicker().pickImage(
                        source: ImageSource.gallery,
                      );
                      if (picked != null) {
                        setStateDialog(
                          () => selectedAvatar = File(picked.path),
                        );
                      }
                    },
                    child: CircleAvatar(
                      radius: 45,
                      backgroundImage: selectedAvatar != null
                          ? FileImage(selectedAvatar!)
                          : NetworkImage(_buildAvatarUrl(child?.anhDaiDien))
                                as ImageProvider,
                      child: const Align(
                        alignment: Alignment.bottomRight,
                        child: Icon(
                          Icons.camera_alt,
                          size: 22,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 12),

                  // Họ tên
                  TextField(
                    controller: nameCtrl,
                    decoration: const InputDecoration(labelText: "Họ tên trẻ"),
                  ),
                  const SizedBox(height: 8),

                  // Giới tính
                  DropdownButtonFormField<int>(
                    value: gioiTinh,
                    decoration: const InputDecoration(labelText: "Giới tính"),
                    items: const [
                      DropdownMenuItem(value: 1, child: Text("Nam")),
                      DropdownMenuItem(value: 0, child: Text("Nữ")),
                      DropdownMenuItem(value: 2, child: Text("Khác")),
                    ],
                    onChanged: (v) => setStateDialog(() => gioiTinh = v!),
                  ),
                  const SizedBox(height: 8),

                  // Ngày sinh
                  ListTile(
                    title: Text(
                      "Ngày sinh: ${birthDate?.toLocal().toString().split(' ')[0] ?? 'Chưa chọn'}",
                    ),
                    trailing: const Icon(Icons.calendar_month),
                    onTap: () async {
                      final date = await showDatePicker(
                        context: context,
                        initialDate: birthDate ?? DateTime(2015),
                        firstDate: DateTime(2000),
                        lastDate: DateTime.now(),
                      );
                      if (date != null) setStateDialog(() => birthDate = date);
                    },
                  ),

                  // Các field chỉ hiển thị khi thêm mới
                  if (child == null) ...[
                    const SizedBox(height: 8),
                    // Số điện thoại (hiển thị cả khi cập nhật)
                    TextField(
                      controller: phoneCtrl,
                      decoration: const InputDecoration(
                        labelText: "Số điện thoại",
                      ),
                      keyboardType: TextInputType.phone,
                    ),
                    TextField(
                      controller: emailCtrl,
                      decoration: const InputDecoration(labelText: "Email trẻ"),
                      keyboardType: TextInputType.emailAddress,
                    ),
                    TextField(
                      controller: passCtrl,
                      decoration: const InputDecoration(labelText: "Mật khẩu"),
                      obscureText: true,
                    ),
                    TextField(
                      controller: confirmCtrl,
                      decoration: const InputDecoration(
                        labelText: "Xác nhận mật khẩu",
                      ),
                      obscureText: true,
                    ),
                  ],
                ],
              ),
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context),
                child: const Text("Hủy"),
              ),
              ElevatedButton(
                onPressed: () async {
                  if (_token == null || _userId == null) return;

                  if (child == null) {
                    // Tạo mới (POST /CreateChild)
                    final fields = {
                      "ParentId": _userId!,
                      "FullName": nameCtrl.text.trim(),
                      "GioiTinh": gioiTinh.toString(),
                      "NgaySinh": birthDate?.toIso8601String() ?? "",
                      "Email": emailCtrl.text.trim(),
                      "PhoneNumber": phoneCtrl.text.trim(),
                      "Password": passCtrl.text.trim(),
                      "ConfirmPassword": confirmCtrl.text.trim(),
                    };

                    // Gửi kèm ảnh nếu có
                    final res = await ChildService.createChild(
                      fields,
                      _token!,
                      avatar: selectedAvatar,
                    );

                    ScaffoldMessenger.of(context).showSnackBar(
                      SnackBar(
                        content: Text(res["message"] ?? "Tạo trẻ thành công"),
                      ),
                    );
                  } else {
                    // Cập nhật (PUT /UpdateChild)
                    final fields = {
                      "ChildrenId": child.childId,
                      "NameND": nameCtrl.text.trim(),
                      "GioiTinh": gioiTinh.toString(),
                      "NgaySinh": birthDate?.toIso8601String() ?? "",
                      "TrangThai": child.trangThai,
                      "UserName":
                          (child.userName?.replaceAll(
                                    RegExp(r'[^a-zA-Z0-9]'),
                                    '',
                                  ) ??
                                  child.childId.replaceAll('-', ''))
                              .substring(0, 12),
                    };

                    final res = await ChildService.updateChild(fields, _token!);
                    ScaffoldMessenger.of(context).showSnackBar(
                      SnackBar(
                        content: Text(res["message"] ?? "Cập nhật thành công"),
                      ),
                    );
                  }

                  Navigator.pop(context);
                  _loadChildren();
                },
                child: Text(child == null ? "Tạo mới" : "Lưu"),
              ),
            ],
          );
        },
      ),
    );
  }

  /// 🗑Xóa trẻ
  Future<void> _deleteChild(Child child) async {
    if (_token == null) return;
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xóa trẻ"),
        content: Text("Bạn có chắc muốn xóa ${child.hoTen}?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text("Hủy"),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text("Xóa"),
          ),
        ],
      ),
    );

    if (confirm == true) {
      final result = await ChildService.deleteChild(child.childId, _token!);
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(result["message"])));
      _loadChildren();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Quản lý trẻ em"),
        backgroundColor: Colors.indigo,
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => _showChildDialog(),
          ),
        ],
      ),
      body: FutureBuilder<ChildrenResponse>(
        future: _futureChildren,
        builder: (context, snapshot) {
          if (!snapshot.hasData) {
            return const Center(child: CircularProgressIndicator());
          }

          final children = snapshot.data!.data;
          if (children.isEmpty) {
            return const Center(child: Text("Chưa có trẻ nào."));
          }

          return RefreshIndicator(
            onRefresh: _loadChildren,
            child: ListView.builder(
              padding: const EdgeInsets.all(12),
              itemCount: children.length,
              itemBuilder: (context, index) {
                final c = children[index];
                return Card(
                  margin: const EdgeInsets.symmetric(vertical: 8),
                  child: ListTile(
                    leading: CircleAvatar(
                      radius: 30,
                      backgroundImage: c.anhDaiDien.isNotEmpty
                          ? NetworkImage(
                              "${dotenv.env['URL_SERVER']}${c.anhDaiDien}",
                            )
                          : const AssetImage("assets/default_avatar.png")
                                as ImageProvider,
                    ),
                    title: Text(c.hoTen),
                    subtitle: Text("👶 ${c.nhomTuoi} | 🚻 ${c.gioiTinh}"),
                    trailing: Wrap(
                      children: [
                        IconButton(
                          icon: const Icon(Icons.edit, color: Colors.orange),
                          onPressed: () => _showChildDialog(child: c),
                        ),
                        IconButton(
                          icon: const Icon(Icons.delete, color: Colors.red),
                          onPressed: () => _deleteChild(c),
                        ),
                      ],
                    ),
                    onTap: () async {
                      final prefs = await SharedPreferences.getInstance();
                      final token = prefs.getString("jwt_token");
                      if (token == null) return;

                      final currentUserId = JwtHelper.getUserId(token);
                      if (currentUserId == null) return;

                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => ChangeNotifierProvider(
                            create: (_) => MessageDetailController(),
                            child: MessageDetailScreen(
                              currentUserId: currentUserId,
                              otherUserId: c.childId, // id của trẻ
                              otherUserName: c.hoTen,
                              otherUserAvatar: c.anhDaiDien
                            ),
                          ),
                        ),
                      );
                    },
                  ),
                );
              },
            ),
          );
        },
      ),
    );
  }
}
