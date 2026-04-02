import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/exam_schedule_model.dart';
import 'package:frontend_qlte/services/exam_schedule_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:intl/intl.dart';

class ExamScheduleChildrenScreen extends StatefulWidget {
  const ExamScheduleChildrenScreen({super.key});

  @override
  State<ExamScheduleChildrenScreen> createState() =>
      _ExamScheduleChildrenScreenState();
}

class _ExamScheduleChildrenScreenState
    extends State<ExamScheduleChildrenScreen> {
  List<ExamSchedule> _examSchedules = [];
  bool _isLoading = true;
  String? _token;
  String? _childId;

  @override
  void initState() {
    super.initState();
    _loadExamSchedules();
  }

  /// 🟢 Lấy danh sách lịch thi
  Future<void> _loadExamSchedules() async {
    setState(() => _isLoading = true);

    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");
    _childId = prefs.getString("child_id");

    if (_childId == null && _token != null) {
      final decoded = JwtHelper.getUserId(_token!);
      if (decoded != null) {
        _childId = decoded;
        await prefs.setString("child_id", _childId!);
      }
    }

    if (_token == null || _childId == null) {
      setState(() => _isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Thiếu token hoặc childId — vui lòng đăng nhập lại.")),
      );
      return;
    }

    final result = await ExamScheduleService.getAllByChild(_childId!, _token!);
    if (!mounted) return;

    if (result["success"] == true) {
      setState(() {
        _examSchedules = List<ExamSchedule>.from(result["data"]);
      });
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result["message"] ?? "Không thể tải dữ liệu.")),
      );
    }

    setState(() => _isLoading = false);
  }

  /// 🟡 Làm mới
  Future<void> _refresh() async => _loadExamSchedules();

  /// 🧮 Nhóm lịch thi theo ngày
  Map<String, List<ExamSchedule>> _groupByDate(List<ExamSchedule> exams) {
    Map<String, List<ExamSchedule>> grouped = {};
    final dateFormat = DateFormat('dd/MM/yyyy');
    for (var exam in exams) {
      final dateKey = dateFormat.format(exam.thoiGianThi);
      grouped.putIfAbsent(dateKey, () => []).add(exam);
    }
    return grouped;
  }

  /// 🧭 Giao diện chính
  @override
  Widget build(BuildContext context) {
    final themeColor = Colors.indigoAccent.shade400;

    return Scaffold(
      backgroundColor: Colors.indigo.shade50,
      appBar: AppBar(
        title: const Text("📘 Lịch thi của con"),
        backgroundColor: themeColor,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _refresh,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _examSchedules.isEmpty
          ? const Center(
        child: Text(
          "Chưa có lịch thi nào được tạo.",
          style: TextStyle(fontSize: 16, color: Colors.black54),
        ),
      )
          : RefreshIndicator(
        onRefresh: _refresh,
        child: _buildGroupedExamList(),
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: themeColor,
        onPressed: () => _showExamDialog(context, title: "Thêm lịch thi"),
        child: const Icon(Icons.add),
      ),
    );
  }

  /// 🧱 Hiển thị danh sách theo nhóm ngày thi
  Widget _buildGroupedExamList() {
    final grouped = _groupByDate(_examSchedules);
    final sortedKeys = grouped.keys.toList()
      ..sort((a, b) =>
          DateFormat('dd/MM/yyyy').parse(a).compareTo(DateFormat('dd/MM/yyyy').parse(b)));

    return ListView.builder(
      padding: const EdgeInsets.all(12),
      itemCount: sortedKeys.length,
      itemBuilder: (context, index) {
        final date = sortedKeys[index];
        final exams = grouped[date]!;

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding:
              const EdgeInsets.symmetric(vertical: 6.0, horizontal: 8.0),
              child: Text(
                "📅 $date",
                style: TextStyle(
                  fontSize: 17,
                  fontWeight: FontWeight.bold,
                  color: Colors.indigo.shade800,
                ),
              ),
            ),
            ...exams.map((exam) => _buildExamCard(exam)).toList(),
          ],
        );
      },
    );
  }

  /// 🧾 Card hiển thị từng môn thi
  Widget _buildExamCard(ExamSchedule exam) {
    final dateFormat = DateFormat('HH:mm dd/MM/yyyy');

    return Card(
      elevation: 3,
      margin: const EdgeInsets.symmetric(vertical: 6, horizontal: 4),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor:
          exam.daThiXong ? Colors.greenAccent.shade400 : Colors.indigo,
          child: const Icon(Icons.edit_calendar, color: Colors.white),
        ),
        title: Text(
          exam.monThi,
          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
        ),
        subtitle: Text(
          "🕓 ${dateFormat.format(exam.thoiGianThi)}"
              "${exam.ghiChu != null ? "\n📝 ${exam.ghiChu}" : ""}",
        ),
        trailing: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            IconButton(
              tooltip: "Sửa lịch thi",
              icon: const Icon(Icons.edit, color: Colors.blueAccent),
              onPressed: () => _showExamDialog(context,
                  title: "Sửa lịch thi", existing: exam),
            ),
            IconButton(
              tooltip: "Xoá lịch thi",
              icon: const Icon(Icons.delete, color: Colors.redAccent),
              onPressed: () => _confirmDeleteExam(exam),
            ),
          ],
        ),
      ),
    );
  }

  /// 🧱 Dialog thêm / sửa lịch thi
  void _showExamDialog(BuildContext context,
      {required String title, ExamSchedule? existing}) {
    final subjectController = TextEditingController(text: existing?.monThi ?? "");
    final dateController = TextEditingController(
        text: existing != null
            ? DateFormat('yyyy-MM-dd HH:mm').format(existing.thoiGianThi)
            : "");
    final noteController = TextEditingController(text: existing?.ghiChu ?? "");
    bool isDone = existing?.daThiXong ?? false;

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(title),
        content: SingleChildScrollView(
          child: Column(
            children: [
              TextField(
                controller: subjectController,
                decoration: const InputDecoration(labelText: "Môn thi"),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: dateController,
                readOnly: true,
                decoration: const InputDecoration(
                    labelText: "Thời gian thi (chọn ngày & giờ)"),
                onTap: () async {
                  final pickedDate = await showDatePicker(
                    context: context,
                    initialDate: existing?.thoiGianThi ?? DateTime.now(),
                    firstDate: DateTime(2024),
                    lastDate: DateTime(2100),
                  );
                  if (pickedDate != null) {
                    final pickedTime = await showTimePicker(
                      context: context,
                      initialTime: TimeOfDay.fromDateTime(
                          existing?.thoiGianThi ?? DateTime.now()),
                    );
                    if (pickedTime != null) {
                      final dateTime = DateTime(
                        pickedDate.year,
                        pickedDate.month,
                        pickedDate.day,
                        pickedTime.hour,
                        pickedTime.minute,
                      );
                      dateController.text =
                          DateFormat('yyyy-MM-dd HH:mm').format(dateTime);
                    }
                  }
                },
              ),
              const SizedBox(height: 8),
              TextField(
                controller: noteController,
                decoration: const InputDecoration(labelText: "Ghi chú"),
              ),
              const SizedBox(height: 8),
              CheckboxListTile(
                value: isDone,
                onChanged: (v) => setState(() => isDone = v ?? false),
                title: const Text("Đã thi xong"),
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
              if (_token == null || _childId == null) return;
              final monThi = subjectController.text.trim();
              if (monThi.isEmpty || dateController.text.isEmpty) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text("Vui lòng nhập đủ thông tin.")),
                );
                return;
              }

              final thoiGianThi =
              DateFormat('yyyy-MM-dd HH:mm').parse(dateController.text);

              Map<String, dynamic> result;
              if (existing == null) {
                result = await ExamScheduleService.createExamSchedule(
                  childId: _childId!,
                  monThi: monThi,
                  thoiGianThi: thoiGianThi,
                  ghiChu: noteController.text.trim(),
                  token: _token!,
                );
              } else {
                result = await ExamScheduleService.updateExamSchedule(
                  examId: existing.examId,
                  monThi: monThi,
                  thoiGianThi: thoiGianThi,
                  ghiChu: noteController.text.trim(),
                  daThiXong: isDone,
                  token: _token!,
                );
              }

              if (!mounted) return;

              // ✅ Đóng dialog trước
              Navigator.pop(context);

              // ✅ Hiển thị thông báo
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(result["message"] ?? "Đã xử lý xong")),
              );

              // ✅ Dù success hay không, vẫn refresh để đồng bộ UI với DB
              await _loadExamSchedules();
            },
            child: const Text("Lưu"),
          ),
        ],
      ),
    );
  }

  /// 🔴 Xác nhận xoá lịch thi
  void _confirmDeleteExam(ExamSchedule exam) {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xoá lịch thi"),
        content: Text("Bạn có chắc muốn xoá môn '${exam.monThi}' không?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Huỷ"),
          ),
          ElevatedButton(
            onPressed: () async {
              if (_token == null) return;
              final result = await ExamScheduleService.deleteExamSchedule(
                exam.examId,
                _token!,
              );

              if (!mounted) return;
              Navigator.pop(context);
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(result["message"] ?? "Không thể xoá")),
              );

              if (result["success"] == true) _loadExamSchedules();
            },
            child: const Text("Xoá"),
          ),
        ],
      ),
    );
  }
}
