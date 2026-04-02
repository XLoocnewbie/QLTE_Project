import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/schedule_model.dart';
import 'package:frontend_qlte/models/child_response_model.dart';
import 'package:frontend_qlte/services/schedule_service.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:intl/intl.dart';

class ScheduleParentScreen extends StatefulWidget {
  const ScheduleParentScreen({super.key});

  @override
  State<ScheduleParentScreen> createState() => _ScheduleParentScreenState();
}

class _ScheduleParentScreenState extends State<ScheduleParentScreen> {
  List<ScheduleModel> _schedules = [];
  List<Child> _children = [];
  String? _selectedChildId;
  bool _isLoading = true;
  String? _token;

  @override
  void initState() {
    super.initState();
    _loadInitialData();
  }

  /// 🧩 Tải danh sách con & token
  Future<void> _loadInitialData() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");

    if (_token == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Thiếu token — vui lòng đăng nhập lại.")),
      );
      setState(() => _isLoading = false);
      return;
    }

    // 🔹 Lấy userId từ token (hoặc từ prefs)
    String? userId = prefs.getString("user_id");
    userId ??= JwtHelper.getUserId(_token!);

    if (userId == null || userId.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Không tìm thấy UserId — vui lòng đăng nhập lại.")),
      );
      setState(() => _isLoading = false);
      return;
    }

    final response = await ChildService.getChildrenByUserId(userId);

    if (response.status == true) {
      setState(() {
        _children = response.data;
        if (_children.isNotEmpty) _selectedChildId = _children.first.childId;
      });
      if (_selectedChildId != null) _loadSchedules(_selectedChildId!);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(response.msg.isNotEmpty ? response.msg : "Không tải được danh sách con")),
      );
      setState(() => _isLoading = false);
    }
  }

  /// 🟢 Lấy danh sách thời khoá biểu theo ChildId
  Future<void> _loadSchedules(String childId) async {
    setState(() => _isLoading = true);

    final result = await ScheduleService.getAllByChild(
      childId: childId,
      token: _token!,
    );

    if (!mounted) return;

    if (result["success"] == true) {
      setState(() {
        _schedules = List<ScheduleModel>.from(result["data"]);
      });
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result["message"] ?? "Không thể tải dữ liệu.")),
      );
    }

    setState(() => _isLoading = false);
  }

  /// 🟣 Chuyển thứ trong tuần
  String _convertDayOfWeek(int day) {
    const days = [
      "Chủ nhật",
      "Thứ hai",
      "Thứ ba",
      "Thứ tư",
      "Thứ năm",
      "Thứ sáu",
      "Thứ bảy"
    ];
    return (day >= 0 && day < days.length) ? days[day] : "Không rõ";
  }

  /// 🔹 Nhóm lịch học theo thứ
  Map<int, List<ScheduleModel>> _groupSchedulesByDay() {
    final Map<int, List<ScheduleModel>> grouped = {};
    for (final s in _schedules) {
      grouped.putIfAbsent(s.thu, () => []).add(s);
    }
    for (final list in grouped.values) {
      list.sort((a, b) => a.gioBatDau.compareTo(b.gioBatDau));
    }
    return grouped;
  }

  /// 🧭 Giao diện chính
  @override
  Widget build(BuildContext context) {
    final themeColor = Colors.blueAccent.shade700;
    final grouped = _groupSchedulesByDay();

    return Scaffold(
      backgroundColor: Colors.blue.shade50,
      appBar: AppBar(
        title: const Text("📅 Quản lý thời khoá biểu"),
        backgroundColor: themeColor,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              if (_selectedChildId != null) _loadSchedules(_selectedChildId!);
            },
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Column(
        children: [
          if (_children.isNotEmpty)
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: DropdownButtonFormField<String>(
                value: _selectedChildId,
                items: _children
                    .map((c) => DropdownMenuItem(
                  value: c.childId,
                  child: Text(c.hoTen),
                ))
                    .toList(),
                decoration: const InputDecoration(
                  labelText: "Chọn trẻ",
                  prefixIcon: Icon(Icons.child_care),
                  border: OutlineInputBorder(),
                ),
                onChanged: (v) {
                  setState(() => _selectedChildId = v);
                  if (v != null) _loadSchedules(v);
                },
              ),
            ),
          Expanded(
            child: _schedules.isEmpty
                ? const Center(
              child: Text(
                "Chưa có thời khoá biểu nào.",
                style:
                TextStyle(fontSize: 16, color: Colors.black54),
              ),
            )
                : RefreshIndicator(
              onRefresh: () => _loadSchedules(_selectedChildId!),
              child: ListView(
                padding: const EdgeInsets.all(12),
                children: grouped.entries.map((entry) {
                  final day = entry.key;
                  final schedules = entry.value;
                  return Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Padding(
                        padding: const EdgeInsets.symmetric(
                            vertical: 8.0),
                        child: Text(
                          "📘 ${_convertDayOfWeek(day)}",
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                            color: Colors.blueAccent,
                          ),
                        ),
                      ),
                      ...schedules.map((s) => Card(
                        elevation: 3,
                        color: Colors.white,
                        margin: const EdgeInsets.symmetric(
                            vertical: 6),
                        shape: RoundedRectangleBorder(
                            borderRadius:
                            BorderRadius.circular(12)),
                        child: ListTile(
                          leading: const Icon(Icons.book,
                              color: Colors.blueAccent),
                          title: Text(
                            s.tenMonHoc,
                            style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 16),
                          ),
                          subtitle: Text(
                            "🕒 ${s.gioBatDau.substring(0, 5)} - ${s.gioKetThuc.substring(0, 5)}",
                            style:
                            const TextStyle(fontSize: 14),
                          ),
                          trailing: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(
                                icon: const Icon(Icons.edit,
                                    color: Colors.orangeAccent),
                                onPressed: () =>
                                    _showEditScheduleDialog(
                                        context, s),
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete,
                                    color: Colors.redAccent),
                                onPressed: () =>
                                    _confirmDeleteSchedule(s),
                              ),
                            ],
                          ),
                        ),
                      )),
                    ],
                  );
                }).toList(),
              ),
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: themeColor,
        onPressed: _selectedChildId == null
            ? null
            : () => _showScheduleDialog(context, _selectedChildId!),
        child: const Icon(Icons.add),
      ),
    );
  }

  /// 🟡 Dialog sửa lịch
  void _showEditScheduleDialog(BuildContext context, ScheduleModel schedule) {
    _showScheduleDialog(context, _selectedChildId!, existing: schedule);
  }

  /// 🧱 Hàm thêm/sửa lịch học
  void _showScheduleDialog(BuildContext context, String childId,
      {ScheduleModel? existing}) {
    final subjectController =
    TextEditingController(text: existing?.tenMonHoc ?? "");
    final startController =
    TextEditingController(text: existing?.gioBatDau.substring(0, 5) ?? "");
    final endController =
    TextEditingController(text: existing?.gioKetThuc.substring(0, 5) ?? "");
    int selectedDay = existing?.thu ?? 1;

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title:
        Text(existing == null ? "Thêm thời khoá biểu" : "Sửa thời khoá biểu"),
        content: SingleChildScrollView(
          child: Column(
            children: [
              TextField(
                controller: subjectController,
                decoration: const InputDecoration(labelText: "Tên môn học"),
              ),
              const SizedBox(height: 8),
              DropdownButtonFormField<int>(
                value: selectedDay,
                items: List.generate(
                  7,
                      (i) => DropdownMenuItem(
                    value: i,
                    child: Text(_convertDayOfWeek(i)),
                  ),
                ),
                onChanged: (v) => selectedDay = v ?? 1,
                decoration: const InputDecoration(labelText: "Thứ"),
              ),
              const SizedBox(height: 8),

              /// 🕒 Giờ bắt đầu
              ListTile(
                title: const Text("Giờ bắt đầu"),
                subtitle: Text(startController.text.isEmpty
                    ? "Chưa chọn"
                    : startController.text),
                trailing: const Icon(Icons.access_time, color: Colors.blueAccent),
                onTap: () async {
                  final picked = await showTimePicker(
                    context: context,
                    initialTime: TimeOfDay.now(),
                  );
                  if (picked != null) {
                    // ✅ Chuẩn 24h format
                    final hour = picked.hour.toString().padLeft(2, '0');
                    final minute = picked.minute.toString().padLeft(2, '0');
                    startController.text = "$hour:$minute:00";
                    setState(() {});
                  }
                },
              ),
              const SizedBox(height: 8),

              /// 🕓 Giờ kết thúc
              ListTile(
                title: const Text("Giờ kết thúc"),
                subtitle: Text(endController.text.isEmpty
                    ? "Chưa chọn"
                    : endController.text),
                trailing: const Icon(Icons.access_time, color: Colors.blueAccent),
                onTap: () async {
                  final picked = await showTimePicker(
                    context: context,
                    initialTime: TimeOfDay.now(),
                  );
                  if (picked != null) {
                    final hour = picked.hour.toString().padLeft(2, '0');
                    final minute = picked.minute.toString().padLeft(2, '0');
                    endController.text = "$hour:$minute:00";
                    setState(() {});
                  }
                },
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Huỷ"),
          ),
          ElevatedButton(
            onPressed: () async {
              if (_token == null) return;
              final subject = subjectController.text.trim();
              if (subject.isEmpty) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text("Vui lòng nhập tên môn học")),
                );
                return;
              }

              /// ⚙️ Giữ format "HH:mm:ss" gửi sang backend
              String normalizeTime(String input) {
                final parts = input.split(':');
                final hour = parts[0].padLeft(2, '0');
                final minute = parts.length > 1 ? parts[1].padLeft(2, '0') : '00';
                return "$hour:$minute:00";
              }

              final start = normalizeTime(startController.text.trim());
              final end = normalizeTime(endController.text.trim());

              Map<String, dynamic> result;
              if (existing == null) {
                result = await ScheduleService.createSchedule(
                  childId: childId,
                  tenMonHoc: subject,
                  thu: selectedDay,
                  gioBatDau: start,
                  gioKetThuc: end,
                  token: _token!,
                );
              } else {
                result = await ScheduleService.updateSchedule(
                  scheduleId: existing.scheduleId,
                  childId: childId,
                  tenMonHoc: subject,
                  thu: selectedDay,
                  gioBatDau: start,
                  gioKetThuc: end,
                  token: _token!,
                );
              }

              if (!mounted) return;
              Navigator.pop(context);
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(result["message"] ?? "Lỗi thao tác")),
              );

              if (result["success"] == true && _selectedChildId != null) {
                _loadSchedules(_selectedChildId!);
              }
            },
            child: const Text("Lưu"),
          ),
        ],
      ),
    );
  }

  /// 🔴 Xác nhận xoá
  void _confirmDeleteSchedule(ScheduleModel schedule) {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xoá lịch học"),
        content:
        Text("Bạn có chắc muốn xoá môn '${schedule.tenMonHoc}' không?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Huỷ"),
          ),
          ElevatedButton(
            onPressed: () async {
              if (_token == null) return;
              final result = await ScheduleService.deleteSchedule(
                scheduleId: schedule.scheduleId,
                token: _token!,
              );

              if (!mounted) return;
              Navigator.pop(context);
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(result["message"] ?? "Không thể xoá")),
              );

              if (result["success"] == true && _selectedChildId != null) {
                _loadSchedules(_selectedChildId!);
              }
            },
            child: const Text("Xoá"),
          ),
        ],
      ),
    );
  }
}
