import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/schedule_model.dart';
import 'package:frontend_qlte/services/schedule_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ScheduleChildrenScreen extends StatefulWidget {
  const ScheduleChildrenScreen({super.key});

  @override
  State<ScheduleChildrenScreen> createState() => _ScheduleChildrenScreenState();
}

class _ScheduleChildrenScreenState extends State<ScheduleChildrenScreen> {
  List<ScheduleModel> _schedules = [];
  bool _isLoading = true;
  String? _token;
  String? _childId;

  @override
  void initState() {
    super.initState();
    _loadSchedules();
  }

  /// 🟢 Lấy danh sách thời khoá biểu theo Child
  Future<void> _loadSchedules() async {
    setState(() => _isLoading = true);

    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token");
    _childId = prefs.getString("child_id");

    // Nếu thiếu childId → lấy tạm từ token
    if (_childId == null && _token != null) {
      try {
        final decodedChildId = JwtHelper.getUserId(_token!);
        if (decodedChildId != null) {
          _childId = decodedChildId;
          await prefs.setString("child_id", _childId!);
          print("🧒 Lưu child_id mới từ token: $_childId");
        }
      } catch (e) {
        print("⚠️ Không thể giải mã child_id: $e");
      }
    }

    if (_token == null || _childId == null) {
      setState(() => _isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Thiếu token hoặc childId — vui lòng đăng nhập lại.")),
      );
      return;
    }

    final result = await ScheduleService.getAllByChild(
      childId: _childId!,
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

  /// 🟡 Làm mới
  Future<void> _refresh() async => _loadSchedules();

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
    final themeColor = Colors.redAccent.shade400;
    final grouped = _groupSchedulesByDay();

    return Scaffold(
      backgroundColor: Colors.red.shade50,
      appBar: AppBar(
        title: const Text("📚 Thời khoá biểu của con"),
        backgroundColor: themeColor,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: "Làm mới",
            onPressed: _refresh,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _schedules.isEmpty
          ? const Center(
        child: Text(
          "Chưa có thời khoá biểu nào được tạo.",
          style: TextStyle(fontSize: 16, color: Colors.black54),
        ),
      )
          : RefreshIndicator(
        onRefresh: _refresh,
        child: ListView(
          padding: const EdgeInsets.all(12),
          children: grouped.entries.map((entry) {
            final day = entry.key;
            final schedules = entry.value;

            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Padding(
                  padding:
                  const EdgeInsets.symmetric(vertical: 8.0),
                  child: Text(
                    "📅 ${_convertDayOfWeek(day)}",
                    style: const TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                      color: Colors.redAccent,
                    ),
                  ),
                ),
                ...schedules.map((s) => Card(
                  elevation: 2,
                  color: Colors.white,
                  margin:
                  const EdgeInsets.symmetric(vertical: 5),
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12)),
                  child: ListTile(
                    leading: const Icon(Icons.book,
                        color: Colors.redAccent),
                    title: Text(
                      s.tenMonHoc,
                      style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 16),
                    ),
                    subtitle: Text(
                      "🕓 ${s.gioBatDau.substring(0, 5)} - ${s.gioKetThuc.substring(0, 5)}",
                      style: const TextStyle(fontSize: 14),
                    ),
                  ),
                )),
              ],
            );
          }).toList(),
        ),
      ),
    );
  }
}