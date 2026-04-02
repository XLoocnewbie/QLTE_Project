import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:frontend_qlte/models/child_response_model.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

class AdminParentChildrenScreen extends StatefulWidget {
  final String token;
  final String parentId;
  final String parentName;

  const AdminParentChildrenScreen({
    super.key,
    required this.token,
    required this.parentId,
    required this.parentName,
  });

  @override
  State<AdminParentChildrenScreen> createState() =>
      _AdminParentChildrenScreenState();
}

class _AdminParentChildrenScreenState
    extends State<AdminParentChildrenScreen> {
  List<Child> _children = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadChildren();
  }

  /// 🟢 Lấy danh sách trẻ theo ParentId
  Future<void> _loadChildren() async {
    setState(() => _isLoading = true);
    final res =
    await ChildService.getChildrenByUserId(widget.parentId);
    setState(() {
      _children = res.data;
      _isLoading = false;
    });
  }

  /// 🔴 Xóa trẻ
  Future<void> _deleteChild(String childId) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xác nhận xóa"),
        content: const Text("Bạn có chắc muốn xóa trẻ này không?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text("Hủy"),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text("Xóa"),
          ),
        ],
      ),
    );

    if (confirm == true) {
      final res = await ChildService.deleteChild(childId, widget.token);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(res["message"] ?? "")),
      );
      if (res["success"]) _loadChildren();
    }
  }

  /// ✏️ Dialog thêm / sửa trẻ
  Future<void> _showChildDialog({Child? child}) async {
    final nameCtrl = TextEditingController(text: child?.hoTen ?? "");
    final emailCtrl = TextEditingController();
    final phoneCtrl = TextEditingController();
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
      builder: (_) => StatefulBuilder(builder: (context, setStateDialog) {
        return AlertDialog(
          title: Text(
            child == null
                ? "➕ Thêm trẻ mới cho ${widget.parentName}"
                : "✏️ Cập nhật thông tin trẻ",
          ),
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
                        : (child?.anhDaiDien.isNotEmpty ?? false)
                        ? NetworkImage("${dotenv.env['URL_SERVER']}${child!.anhDaiDien}")
                        : const AssetImage("assets/default_avatar.png") as ImageProvider,
                    child: const Align(
                      alignment: Alignment.bottomRight,
                      child: Icon(Icons.camera_alt, size: 22, color: Colors.white),
                    ),
                  ),
                ),
                const SizedBox(height: 12),

                // 👶 Họ tên
                TextField(
                  controller: nameCtrl,
                  decoration: const InputDecoration(labelText: "Họ tên trẻ"),
                ),
                const SizedBox(height: 8),

                // 🚻 Giới tính
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

                // 📅 Ngày sinh
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

                // 🆕 Các field chỉ hiển thị khi thêm mới
                if (child == null) ...[
                  const SizedBox(height: 8),
                  TextField(
                    controller: emailCtrl,
                    decoration: const InputDecoration(labelText: "Email trẻ"),
                    keyboardType: TextInputType.emailAddress,
                  ),
                  TextField(
                    controller: phoneCtrl,
                    decoration: const InputDecoration(labelText: "Số điện thoại"),
                    keyboardType: TextInputType.phone,
                  ),
                  TextField(
                    controller: passCtrl,
                    decoration: const InputDecoration(labelText: "Mật khẩu"),
                    obscureText: true,
                  ),
                  TextField(
                    controller: confirmCtrl,
                    decoration:
                    const InputDecoration(labelText: "Xác nhận mật khẩu"),
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
                if (child == null) {
                  // 🟢 Tạo mới
                  final fields = {
                    "ParentId": widget.parentId,
                    "FullName": nameCtrl.text.trim(),
                    "GioiTinh": gioiTinh.toString(),
                    "NgaySinh": birthDate?.toIso8601String() ?? "",
                    "Email": emailCtrl.text.trim(),
                    "PhoneNumber": phoneCtrl.text.trim(),
                    "Password": passCtrl.text.trim(),
                    "ConfirmPassword": confirmCtrl.text.trim(),
                  };
                  final res = await ChildService.createChild(
                    fields,
                    widget.token,
                    avatar: selectedAvatar,
                  );
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(content: Text(res["message"] ?? "")),
                  );
                } else {
                  // 🟠 Cập nhật
                  final fields = {
                    "ChildrenId": child.childId,
                    "NameND": nameCtrl.text.trim(),
                    "GioiTinh": gioiTinh.toString(),
                    "NgaySinh": birthDate?.toIso8601String() ?? "",
                    "TrangThai": child.trangThai,
                    "UserName": (child.userName
                        ?.replaceAll(RegExp(r'[^a-zA-Z0-9]'), '') ??
                        child.childId.replaceAll('-', ''))
                        .substring(0, 12),
                  };
                  final res = await ChildService.updateChild(fields, widget.token);
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(content: Text(res["message"] ?? "")),
                  );
                }

                Navigator.pop(context);
                _loadChildren();
              },
              child: Text(child == null ? "Tạo mới" : "Lưu"),
            ),
          ],
        );
      }),
    );
  }

  /// 🖥️ Hiển thị danh sách trẻ
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("👨 ${widget.parentName}"),
        actions: [
          IconButton(
            onPressed: _loadChildren,
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _children.isEmpty
          ? const Center(child: Text("Không có trẻ nào trong danh sách."))
          : ListView.builder(
        itemCount: _children.length,
        itemBuilder: (context, index) {
          final c = _children[index];
          return Card(
            margin: const EdgeInsets.symmetric(
                horizontal: 12, vertical: 6),
            child: ListTile(
              leading: CircleAvatar(
                backgroundImage: (c.anhDaiDien?.isNotEmpty ?? false)
                    ? NetworkImage("${dotenv.env['URL_SERVER']}${c.anhDaiDien}")
                    : const AssetImage("assets/default_avatar.png") as ImageProvider,
              ),
              title: Text(c.hoTen,
                  style:
                  const TextStyle(fontWeight: FontWeight.bold)),
              subtitle: Text(
                  "Giới tính: ${c.gioiTinh} • Nhóm tuổi: ${c.nhomTuoi}"),
              trailing: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: const Icon(Icons.edit, color: Colors.blue),
                    onPressed: () => _showChildDialog(child: c),
                  ),
                  IconButton(
                    icon: const Icon(Icons.delete, color: Colors.red),
                    onPressed: () => _deleteChild(c.childId),
                  ),
                ],
              ),
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showChildDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
