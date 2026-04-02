import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/device_restriction_model.dart';
import 'package:frontend_qlte/services/device_restriction_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class RestrictionParentScreen extends StatefulWidget {
  const RestrictionParentScreen({super.key});

  @override
  State<RestrictionParentScreen> createState() => _RestrictionParentScreenState();
}

class _RestrictionParentScreenState extends State<RestrictionParentScreen> {
  List<DeviceRestrictionModel> _restrictions = [];
  bool _isLoading = true;
  String? _deviceId;

  @override
  void initState() {
    super.initState();
    _loadRestrictions();
  }

  Future<void> _loadRestrictions() async {
    setState(() => _isLoading = true);

    final prefs = await SharedPreferences.getInstance();
    _deviceId = prefs.getString("device_id");

    if (_deviceId == null || _deviceId!.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("⚠️ Không tìm thấy DeviceId, vui lòng đăng nhập lại."),
          backgroundColor: Colors.orange,
        ),
      );
      setState(() => _isLoading = false);
      return;
    }

    // 🟢 Thêm log ở đây
    print("🔍 Đang tải danh sách hạn chế cho deviceId=$_deviceId");

    try {
      final list = await DeviceRestrictionService.getAllByDevice(_deviceId!);
      setState(() => _restrictions = list);

      // 🟢 Thêm log sau khi tải xong
      print("✅ Tải thành công ${list.length} cấu hình hạn chế.");
    } catch (e) {
      print("❌ Lỗi khi tải danh sách hạn chế: $e");
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Lỗi tải danh sách: $e"), backgroundColor: Colors.red),
      );
    }

    setState(() => _isLoading = false);
  }

  Future<void> _toggleFirewall(String restrictionId) async {
    try {
      final msg = await DeviceRestrictionService.toggleFirewall(restrictionId);
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
      _loadRestrictions();
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Lỗi bật/tắt Firewall: $e"), backgroundColor: Colors.red),
      );
    }
  }

  Future<void> _deleteRestriction(String restrictionId) async {
    try {
      final msg = await DeviceRestrictionService.delete(restrictionId);
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
      _loadRestrictions();
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Lỗi xóa cấu hình: $e"), backgroundColor: Colors.red),
      );
    }
  }

  /// 🧩 Form thêm/sửa cấu hình hạn chế
  Future<void> _openRestrictionForm({DeviceRestrictionModel? existing}) async {
    List<String> blockedApps = existing?.blockedApps?.split(',') ?? [];
    final blockedDomainController = TextEditingController(text: existing?.blockedDomains ?? "");
    final allowedDomainController = TextEditingController(text: existing?.allowedDomains ?? "");
    String mode = existing?.mode ?? "Custom";

    Future<void> _addBlockedApp() async {
      final controller = TextEditingController();
      await showDialog(
        context: context,
        builder: (_) => AlertDialog(
          title: const Text("➕ Thêm ứng dụng cần chặn"),
          content: TextField(
            controller: controller,
            decoration: const InputDecoration(
              hintText: "Nhập tên ứng dụng (VD: YouTube, TikTok, Facebook)",
            ),
          ),
          actions: [
            TextButton(onPressed: () => Navigator.pop(context), child: const Text("Huỷ")),
            ElevatedButton(
              onPressed: () {
                final app = controller.text.trim();
                if (app.isNotEmpty) {
                  setState(() => blockedApps.add(app));
                }
                Navigator.pop(context);
              },
              child: const Text("Thêm"),
            ),
          ],
        ),
      );
    }

    await showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (_) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(context).viewInsets.bottom,
          left: 20,
          right: 20,
          top: 20,
        ),
        child: StatefulBuilder(builder: (context, setModalState) {
          return SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  existing == null ? "➕ Thêm cấu hình hạn chế" : "✏️ Sửa cấu hình hạn chế",
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
                ),
                const SizedBox(height: 16),

                // 🔹 Danh sách app bị chặn
                const Text("Ứng dụng bị chặn:", style: TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 8),
                Wrap(
                  spacing: 6,
                  children: blockedApps.map((app) {
                    return Chip(
                      label: Text(app),
                      deleteIcon: const Icon(Icons.close),
                      onDeleted: () {
                        setModalState(() => blockedApps.remove(app));
                      },
                    );
                  }).toList(),
                ),
                TextButton.icon(
                  onPressed: _addBlockedApp,
                  icon: const Icon(Icons.add),
                  label: const Text("Thêm ứng dụng"),
                ),
                const SizedBox(height: 12),

                // 🔹 Website bị chặn
                TextField(
                  controller: blockedDomainController,
                  decoration: const InputDecoration(labelText: "Website bị chặn (ngăn cách bằng ,)"),
                ),
                const SizedBox(height: 12),

                // 🔹 Website được phép
                TextField(
                  controller: allowedDomainController,
                  decoration: const InputDecoration(labelText: "Website được phép (ngăn cách bằng ,)"),
                ),
                const SizedBox(height: 12),

                // 🔹 Chế độ hạn chế
                DropdownButtonFormField<String>(
                  value: mode,
                  decoration: const InputDecoration(labelText: "Chế độ hạn chế"),
                  items: const [
                    DropdownMenuItem(value: "Custom", child: Text("Tuỳ chỉnh")),
                    DropdownMenuItem(value: "StudyMode", child: Text("Giờ học")),
                    DropdownMenuItem(value: "EmergencyLock", child: Text("Khoá khẩn cấp")),
                  ],
                  onChanged: (v) => setModalState(() => mode = v!),
                ),
                const SizedBox(height: 24),

                // 🔹 Nút lưu
                Center(
                  child: ElevatedButton.icon(
                    icon: const Icon(Icons.save),
                    label: const Text("Lưu cấu hình"),
                    onPressed: () async {
                      final prefs = await SharedPreferences.getInstance();
                      final deviceId = prefs.getString("device_id");
                      final token = prefs.getString("jwt_token");

                      if (deviceId == null || token == null || token.isEmpty) {
                        print("❌ Không tìm thấy device_id hoặc jwt_token, không thể gửi API");
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("⚠️ Thiếu thông tin đăng nhập, vui lòng đăng nhập lại."),
                            backgroundColor: Colors.orange,
                          ),
                        );
                        return;
                      }

                      print("📱 DeviceId: $deviceId");
                      print("🔑 Token available: ${token.substring(0, 15)}...");


                      final model = DeviceRestrictionModel(
                        restrictionId: existing?.restrictionId ?? '',
                        deviceId: deviceId,
                        blockedApps: blockedApps.join(','),
                        blockedDomains: blockedDomainController.text,
                        allowedDomains: allowedDomainController.text,
                        isFirewallEnabled: existing?.isFirewallEnabled ?? false,
                        mode: mode,
                        updatedAt: DateTime.now(),
                      );

                      try {
                        final msg = existing == null
                            ? await DeviceRestrictionService.create(model)
                            : await DeviceRestrictionService.update(model);

                        if (mounted) {
                          Navigator.pop(context);
                          ScaffoldMessenger.of(context)
                              .showSnackBar(SnackBar(content: Text(msg)));
                          _loadRestrictions();

                        }
                      } catch (e) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text("Lỗi lưu cấu hình: $e"),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    },
                  ),
                ),
                const SizedBox(height: 12),
              ],
            ),
          );
        }),
      ),
    );
  }

  /// 🧩 Card hiển thị cấu hình
  Widget _buildRestrictionCard(DeviceRestrictionModel restriction) {
    final isStudy = restriction.mode == "StudyMode";
    final isEmergency = restriction.mode == "EmergencyLock";
    final cardColor = isEmergency
        ? Colors.redAccent.withOpacity(0.1)
        : isStudy
        ? Colors.blueAccent.withOpacity(0.1)
        : Colors.grey.withOpacity(0.05);

    return Card(
      color: cardColor,
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      elevation: 3,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  isStudy
                      ? "📘 Chế độ học tập"
                      : isEmergency
                      ? "🚨 Khoá khẩn cấp"
                      : "⚙️ Tuỳ chỉnh",
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                ),
                Switch(
                  value: restriction.isFirewallEnabled,
                  activeColor: Colors.green,
                  onChanged: (_) => _toggleFirewall(restriction.restrictionId ?? ''),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text("🔒 App bị chặn: ${restriction.blockedApps ?? 'Không có'}"),
            Text("🌐 Web bị chặn: ${restriction.blockedDomains ?? 'Không có'}"),
            const SizedBox(height: 6),
            Text(
              restriction.updatedAt != null
                  ? "🕒 Cập nhật: ${restriction.updatedAt!.toLocal()}"
                  : "🕒 Chưa có thời gian cập nhật",
              style: const TextStyle(color: Colors.black54, fontSize: 12),
            ),
            const Divider(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                IconButton(
                  icon: const Icon(Icons.edit, color: Colors.orange),
                  onPressed: () => _openRestrictionForm(existing: restriction),
                ),
                IconButton(
                  icon: const Icon(Icons.delete, color: Colors.red),
                  onPressed: () => _deleteRestriction(restriction.restrictionId ?? ''),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("🔒 Quản lý hạn chế thiết bị"),
        backgroundColor: Colors.blueAccent,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadRestrictions),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _restrictions.isEmpty
          ? const Center(child: Text("Không có cấu hình hạn chế nào."))
          : RefreshIndicator(
        onRefresh: _loadRestrictions,
        child: ListView.builder(
          itemCount: _restrictions.length,
          itemBuilder: (_, i) => _buildRestrictionCard(_restrictions[i]),
        ),
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: Colors.blueAccent,
        onPressed: () => _openRestrictionForm(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
