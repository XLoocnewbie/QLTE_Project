import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:frontend_qlte/models/parent_with_children_model.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:frontend_qlte/screens/admin/admin_parent_children_screen.dart';

class AdminChildrenScreen extends StatefulWidget {
  final String token;
  const AdminChildrenScreen({super.key, required this.token});

  @override
  State<AdminChildrenScreen> createState() => _AdminChildrenScreenState();
}

class _AdminChildrenScreenState extends State<AdminChildrenScreen> {
  bool _isLoading = false;
  List<ParentWithChildren> _parents = [];

  @override
  void initState() {
    super.initState();
    _loadParents();
  }

  /// 🟢 Gọi API lấy danh sách tất cả phụ huynh + danh sách con của họ
  Future<void> _loadParents() async {
    setState(() => _isLoading = true);

    final res = await ChildService.getAllParentsWithChildren(widget.token);

    if (res["success"] == true) {
      setState(() => _parents = res["data"]);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(res["message"] ?? "Không thể tải danh sách")),
      );
    }

    setState(() => _isLoading = false);
  }

  /// 🧩 Hàm build ảnh đại diện (chuẩn hóa với .env & fallback)
  ImageProvider _buildAvatar(String? avatarPath) {
    final baseUrl = dotenv.env['URL_SERVER'] ?? '';

    // Nếu không có ảnh → dùng mặc định
    if (avatarPath == null || avatarPath.isEmpty) {
      return const AssetImage("assets/default_avatar.png");
    }

    // Nếu đã là đường dẫn HTTP → giữ nguyên
    if (avatarPath.startsWith("http")) {
      return NetworkImage(avatarPath);
    }

    // Chuẩn hóa URL để tránh bị "//"
    final cleanBase = baseUrl.endsWith('/') ? baseUrl : "$baseUrl/";
    final cleanPath = avatarPath.startsWith('/')
        ? avatarPath.substring(1)
        : avatarPath;

    return NetworkImage("$cleanBase$cleanPath");
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("👨‍👩‍👧 Quản lý trẻ em"),
        backgroundColor: Colors.indigo,
        actions: [
          IconButton(
            onPressed: _loadParents,
            icon: const Icon(Icons.refresh),
            tooltip: "Tải lại danh sách",
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _parents.isEmpty
          ? const Center(child: Text("Không có phụ huynh nào trong hệ thống"))
          : ListView.builder(
        itemCount: _parents.length,
        itemBuilder: (context, index) {
          final parent = _parents[index];

          return Card(
            margin: const EdgeInsets.symmetric(
                horizontal: 12, vertical: 6),
            elevation: 2,
            child: ExpansionTile(
              // 🧍 Avatar phụ huynh (tự động từ .env hoặc fallback)
              leading: CircleAvatar(
                radius: 25,
                backgroundImage: (parent.avatar != null && parent.avatar!.isNotEmpty)
                    ? _buildAvatar(parent.avatar)
                    : const AssetImage("assets/default_avatar.png") as ImageProvider,
              ),
              title: Text(
                parent.parentName,
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              subtitle: Text(
                  "${parent.email}\nSĐT: ${parent.phoneNumber}"),
              childrenPadding: const EdgeInsets.symmetric(
                  horizontal: 12, vertical: 4),
              children: [
                if (parent.children.isEmpty)
                  const Padding(
                    padding: EdgeInsets.all(8.0),
                    child: Text("Không có trẻ nào."),
                  )
                else
                  Column(
                    children: parent.children.map((child) {
                      return ListTile(
                        // 👶 Avatar của trẻ (lấy từ .env)
                        leading: CircleAvatar(
                          radius: 22,
                          backgroundImage:
                          _buildAvatar(child.anhDaiDien),
                        ),
                        title: Text(child.hoTen),
                        subtitle: Text(
                          "Giới tính: ${child.gioiTinh} | Nhóm tuổi: ${child.nhomTuoi}",
                        ),
                        trailing: const Icon(
                          Icons.arrow_forward_ios_rounded,
                          size: 14,
                        ),
                        onTap: () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) =>
                                  AdminParentChildrenScreen(
                                    token: widget.token,
                                    parentId: parent.parentId,
                                    parentName: parent.parentName,
                                  ),
                            ),
                          );
                        },
                      );
                    }).toList(),
                  ),
              ],
            ),
          );
        },
      ),
    );
  }
}
