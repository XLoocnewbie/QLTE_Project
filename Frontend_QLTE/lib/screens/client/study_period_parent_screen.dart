import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/study_period_model.dart';
import 'package:frontend_qlte/services/study_period_service.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend_qlte/models/device_restriction_model.dart';
import 'package:frontend_qlte/services/device_restriction_service.dart';

class StudyPeriodParentScreen extends StatefulWidget {
  const StudyPeriodParentScreen({super.key});

  @override
  State<StudyPeriodParentScreen> createState() =>
      _StudyPeriodParentScreenState();
}

class _StudyPeriodParentScreenState extends State<StudyPeriodParentScreen> {
  List<StudyPeriod> _studyPeriods = [];
  bool _isLoading = true;
  String? _childId;
  String? _deviceId;

  @override
  void initState() {
    super.initState();
    _loadStudyPeriods();
  }

  Future<void> _loadStudyPeriods() async {
    setState(() => _isLoading = true);

    final prefs = await SharedPreferences.getInstance();
    _childId = prefs.getString("child_id");
    _deviceId = prefs.getString("device_id");

    if (_childId == null || _childId!.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("⚠️ Không tìm thấy ChildId, vui lòng đăng nhập lại."),
          backgroundColor: Colors.orange,
        ),
      );
      setState(() => _isLoading = false);
      return;
    }

    final result = await StudyPeriodService.getByChild(_childId!);
    if (result["success"] == true) {
      setState(() => _studyPeriods = result["data"]);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result["message"] ?? "Không thể tải danh sách giờ học."),
          backgroundColor: Colors.red,
        ),
      );
    }

    setState(() => _isLoading = false);
  }

  Future<void> _toggleActive(StudyPeriod sp) async {
    final turningOn = !sp.isActive;

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(turningOn
            ? "📘 Đang bật chế độ học cho thiết bị..."
            : "🛑 Đang tắt chế độ học cho thiết bị..."),
        duration: const Duration(seconds: 2),
      ),
    );

    final result = await StudyPeriodService.toggleActive(sp.studyPeriodId);

    if (result["success"] == true) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result["message"] ?? "Cập nhật trạng thái thành công"),
          backgroundColor: Colors.green,
        ),
      );

      // ✅ Cập nhật danh sách bằng copyWith (không sửa trực tiếp field final)
      setState(() {
        _studyPeriods = _studyPeriods.map((item) {
          if (item.studyPeriodId == sp.studyPeriodId) {
            return item.copyWith(isActive: turningOn);
          }
          return item;
        }).toList();
      });
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result["message"] ?? "Không thể cập nhật trạng thái"),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _deleteStudyPeriod(StudyPeriod sp) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xóa khung giờ học"),
        content:
        Text("Bạn có chắc muốn xóa khung giờ ${sp.startTime} → ${sp.endTime}?"),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(context, false),
              child: const Text("Hủy")),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text("Xóa"),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    final result = await StudyPeriodService.delete(sp.studyPeriodId);
    if (result["success"] == true) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result["message"] ?? "Đã xóa thành công")),
      );
      _loadStudyPeriods();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result["message"] ?? "Không thể xóa khung giờ học"),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  // 📆 Form tạo/sửa
  Future<void> _openStudyPeriodForm({StudyPeriod? existing}) async {
    final startController =
    TextEditingController(text: existing?.startTime ?? "");
    final endController = TextEditingController(text: existing?.endTime ?? "");
    final descController = TextEditingController(text: existing?.moTa ?? "");
    String repeat = existing?.repeatPattern ?? "Daily";

    await showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (_) {
        return Padding(
          padding: EdgeInsets.only(
            bottom: MediaQuery.of(context).viewInsets.bottom,
            left: 20,
            right: 20,
            top: 20,
          ),
          child: StatefulBuilder(
            builder: (context, setModalState) {
              Future<void> _pickTime(TextEditingController controller) async {
                final initial = const TimeOfDay(hour: 8, minute: 0);
                final picked = await showTimePicker(
                  context: context,
                  initialTime: initial,
                  builder: (context, child) {
                    return MediaQuery(
                      data: MediaQuery.of(context)
                          .copyWith(alwaysUse24HourFormat: true),
                      child: child!,
                    );
                  },
                );

                if (picked != null) {
                  // ✅ Format đúng chuẩn HH:mm:ss cho backend
                  final formatted = '${picked.hour.toString().padLeft(2, '0')}:'
                      '${picked.minute.toString().padLeft(2, '0')}:00';
                  controller.text = formatted;
                  print('🕒 Picked time => $formatted');
                }
              }

              return SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      existing == null
                          ? "➕ Thêm khung giờ học"
                          : "✏️ Cập nhật khung giờ học",
                      style: const TextStyle(
                          fontSize: 18, fontWeight: FontWeight.bold),
                    ),
                    const SizedBox(height: 12),

                    TextField(
                      controller: startController,
                      readOnly: true,
                      decoration: InputDecoration(
                        labelText: "Giờ bắt đầu",
                        suffixIcon: IconButton(
                          icon: const Icon(Icons.access_time),
                          onPressed: () => _pickTime(startController),
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),

                    TextField(
                      controller: endController,
                      readOnly: true,
                      decoration: InputDecoration(
                        labelText: "Giờ kết thúc",
                        suffixIcon: IconButton(
                          icon: const Icon(Icons.access_time),
                          onPressed: () => _pickTime(endController),
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),

                    TextField(
                      controller: descController,
                      decoration:
                      const InputDecoration(labelText: "Mô tả (tuỳ chọn)"),
                    ),
                    const SizedBox(height: 12),

                    DropdownButtonFormField<String>(
                      value: repeat,
                      items: const [
                        DropdownMenuItem(
                            value: "Daily", child: Text("Hàng ngày")),
                        DropdownMenuItem(
                            value: "Weekday", child: Text("Ngày trong tuần")),
                        DropdownMenuItem(
                            value: "Weekend", child: Text("Cuối tuần")),
                      ],
                      onChanged: (v) => setModalState(() => repeat = v!),
                      decoration:
                      const InputDecoration(labelText: "Kiểu lặp lại"),
                    ),
                    const SizedBox(height: 24),

                    Center(
                      child: ElevatedButton.icon(
                        icon: const Icon(Icons.save),
                        label: Text(
                            existing == null ? "Tạo mới" : "Lưu thay đổi"),
                        onPressed: () async {
                          final start = startController.text.trim();
                          final end = endController.text.trim();
                          if (start.isEmpty || end.isEmpty) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                  content: Text(
                                      "Vui lòng chọn giờ bắt đầu và kết thúc.")),
                            );
                            return;
                          }

                          // ✅ Không convert nữa — dùng giá trị chuẩn HH:mm:ss
                          final startFormatted = start;
                          final endFormatted = end;

                          final result = existing == null
                              ? await StudyPeriodService.create(
                            childId: _childId!,
                            startTime: startFormatted,
                            endTime: endFormatted,
                            repeatPattern: repeat,
                            moTa: descController.text.trim(),
                          )
                              : await StudyPeriodService.update(
                            studyPeriodId: existing.studyPeriodId,
                            startTime: startFormatted,
                            endTime: endFormatted,
                            repeatPattern: repeat,
                            moTa: descController.text.trim(),
                          );

                          if (result["success"] == true) {
                            Navigator.pop(context);
                            _loadStudyPeriods();
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                  content: Text(
                                      result["message"] ?? "Thành công")),
                            );
                          } else {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(
                                content: Text(result["message"] ??
                                    "Lỗi khi lưu dữ liệu."),
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
            },
          ),
        );
      },
    );
  }

  Widget _buildStudyPeriodCard(StudyPeriod sp) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      elevation: 3,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: ListTile(
        contentPadding:
        const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        title: Text(
          "${sp.startTime} → ${sp.endTime}",
          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        subtitle: Text(
          "Lặp lại: ${sp.repeatPattern}\n"
              "Trạng thái: ${sp.isActive ? '🟢 Đang học' : '⚪ Nghỉ'}\n"
              "Mô tả: ${sp.moTa ?? 'Không có'}",
          style: const TextStyle(fontSize: 14, color: Colors.black54),
        ),
        trailing: Wrap(
          spacing: 4,
          children: [
            IconButton(
              icon: const Icon(Icons.edit, color: Colors.orange),
              onPressed: () => _openStudyPeriodForm(existing: sp),
            ),
            IconButton(
              icon: const Icon(Icons.delete, color: Colors.red),
              onPressed: () => _deleteStudyPeriod(sp),
            ),
            Switch(
              value: sp.isActive,
              activeColor: Colors.green,
              onChanged: (_) => _toggleActive(sp),
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
        title: const Text("📘 Quản lý chế độ học của con"),
        backgroundColor: Colors.blueAccent,
        actions: [
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadStudyPeriods),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
        onRefresh: _loadStudyPeriods,
        child: _studyPeriods.isEmpty
            ? const Center(child: Text("Không có khung giờ học nào."))
            : ListView.builder(
          itemCount: _studyPeriods.length,
          itemBuilder: (_, i) =>
              _buildStudyPeriodCard(_studyPeriods[i]),
        ),
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: Colors.blueAccent,
        onPressed: () => _openStudyPeriodForm(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
