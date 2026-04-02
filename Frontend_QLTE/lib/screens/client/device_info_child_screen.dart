import 'dart:math';
import 'package:flutter/material.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:frontend_qlte/services/device_info_service.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:frontend_qlte/models/child_response_model.dart';
import 'package:frontend_qlte/models/device_info_model.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend_qlte/screens/client/device_detail_child_screen.dart';

class DeviceInfoChildScreen extends StatefulWidget {
  const DeviceInfoChildScreen({super.key});

  @override
  State<DeviceInfoChildScreen> createState() => _DeviceInfoChildScreenState();
}

class _DeviceInfoChildScreenState extends State<DeviceInfoChildScreen> {
  final SignalRService _signalRService = SignalRService();
  bool _loading = true;
  List<Child> _children = [];
  Child? _selectedChild;
  List<DeviceInfoModel> _devices = [];

  @override
  void initState() {
    super.initState();
    _loadChildren();
    _initSignalR();
  }

  // 🧩 Lấy danh sách child theo parent
  Future<void> _loadChildren() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    final userId = prefs.getString('user_id');

    if (token == null || userId == null) {
      setState(() => _loading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không tìm thấy thông tin người dùng.')),
      );
      return;
    }

    final result = await ChildService.getChildrenByUserId(userId);
    if (result.status && result.data.isNotEmpty) {
      setState(() {
        _children = result.data;
        _selectedChild = _children.first;
      });
      await _loadDevicesByChild(_selectedChild!.childId);
    } else {
      setState(() => _loading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result.msg)),
      );
    }
  }

  // 🧩 Lấy thiết bị theo ChildId
  Future<void> _loadDevicesByChild(String childId) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    if (token == null) return;

    setState(() => _loading = true);
    final result = await DeviceInfoService.getByChild(childId, token);
    if (result['success'] && result['data'] != null) {
      setState(() {
        _devices = List<DeviceInfoModel>.from(result['data']);
        _loading = false;
      });
    } else {
      setState(() {
        _devices = [];
        _loading = false;
      });
    }
  }

  // 🧩 Khởi tạo SignalR (tùy chọn)
  Future<void> _initSignalR() async {
    try {
      await _signalRService.connectDeviceHub();
      print("✅ DeviceHub connected");
    } catch (e) {
      print("⚠️ Lỗi kết nối DeviceHub: $e");
    }
  }

  // 🧩 Mở dialog thêm thiết bị
  Future<void> _showAddDeviceDialog() async {
    if (_children.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Không có trẻ nào để thêm thiết bị.")),
      );
      return;
    }

    final tenThietBiController = TextEditingController();
    Child? selectedInDialog = _selectedChild;

    await showDialog(
      context: context,
      builder: (ctx) {
        return StatefulBuilder(builder: (context, setDialogState) {
          return AlertDialog(
            title: const Text("➕ Thêm thiết bị cho trẻ"),
            content: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                DropdownButtonFormField<Child>(
                  value: selectedInDialog,
                  hint: const Text("Chọn trẻ em"),
                  items: _children.map((child) {
                    return DropdownMenuItem(
                      value: child,
                      child: Text(child.hoTen),
                    );
                  }).toList(),
                  onChanged: (value) {
                    setDialogState(() => selectedInDialog = value);
                  },
                  decoration: const InputDecoration(
                    border: OutlineInputBorder(),
                    labelText: "Trẻ em",
                  ),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: tenThietBiController,
                  decoration: const InputDecoration(
                    border: OutlineInputBorder(),
                    labelText: "Tên thiết bị",
                  ),
                ),
              ],
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(ctx),
                child: const Text("Huỷ"),
              ),
              ElevatedButton.icon(
                onPressed: () async {
                  if (selectedInDialog == null ||
                      tenThietBiController.text.trim().isEmpty) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                          content:
                          Text("Vui lòng chọn trẻ và nhập tên thiết bị.")),
                    );
                    return;
                  }

                  Navigator.pop(ctx);
                  await _createDeviceForChild(
                    childId: selectedInDialog!.childId,
                    tenThietBi: tenThietBiController.text.trim(),
                  );
                },
                icon: const Icon(Icons.add),
                label: const Text("Tạo thiết bị"),
                style: ElevatedButton.styleFrom(backgroundColor: Colors.teal),
              ),
            ],
          );
        });
      },
    );
  }

  // 🧩 Gọi API tạo thiết bị cho child
  Future<void> _createDeviceForChild({
    required String childId,
    required String tenThietBi,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwt_token');
    if (token == null) return;

    final randomImei = List.generate(15, (_) => Random().nextInt(10)).join('');
    final result = await DeviceInfoService.createDevice(
      childId: childId,
      tenThietBi: tenThietBi,
      imei: randomImei,
      token: token,
    );

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(result['message']),
        backgroundColor: result['success'] ? Colors.green : Colors.redAccent,
      ),
    );

    if (result['success']) {
      await _loadDevicesByChild(childId); // ✅ refresh danh sách sau khi thêm
    }
  }

  @override
  Widget build(BuildContext context) {
    final themeColor = Colors.teal.shade600;

    return Scaffold(
      appBar: AppBar(
        title: const Text("Quản lý thiết bị trẻ em"),
        backgroundColor: themeColor,
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: Colors.teal,
        onPressed: _showAddDeviceDialog,
        label: const Text("Thêm thiết bị"),
        icon: const Icon(Icons.add),
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _children.isEmpty
          ? const Center(child: Text("Không có trẻ nào trong tài khoản của bạn."))
          : Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            // 🔽 Dropdown chọn trẻ
            DropdownButtonFormField<Child>(
              value: _selectedChild,
              hint: const Text("Chọn trẻ để xem thiết bị"),
              items: _children.map((child) {
                return DropdownMenuItem(
                  value: child,
                  child: Text(child.hoTen),
                );
              }).toList(),
              onChanged: (value) async {
                setState(() {
                  _selectedChild = value;
                  _devices.clear();
                });
                await _loadDevicesByChild(value!.childId);
              },
              decoration: const InputDecoration(
                border: OutlineInputBorder(),
                labelText: "Trẻ em",
              ),
            ),
            const SizedBox(height: 20),

            // 📱 Danh sách thiết bị
            Expanded(
              child: _devices.isEmpty
                  ? const Center(
                child: Text(
                  "Chưa có thiết bị nào cho trẻ này.",
                  style: TextStyle(fontSize: 16, color: Colors.grey),
                ),
              )
                  : ListView.builder(
                itemCount: _devices.length,
                itemBuilder: (context, index) {
                  final device = _devices[index];
                  return Card(
                    margin: const EdgeInsets.symmetric(vertical: 8),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                    elevation: 3,
                    child: ListTile(
                      leading: Icon(
                        Icons.phone_android,
                        color: device.trangThaiOnline
                            ? Colors.green
                            : Colors.grey,
                        size: 36,
                      ),
                      title: Text(
                        device.tenThietBi ?? "Thiết bị không tên",
                        style: const TextStyle(
                            fontSize: 18, fontWeight: FontWeight.w600),
                      ),
                      subtitle: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text("IMEI: ${device.imei ?? '---'}"),
                          Text("Pin: ${device.pin ?? '--'}%"),
                          Text(device.isTracking
                              ? "🛰️ Đang theo dõi định kỳ"
                              : "🛰️ Theo dõi tắt"),
                        ],
                      ),
                      trailing: IconButton(
                        icon: const Icon(Icons.refresh,
                            color: Colors.teal),
                        onPressed: () async {
                          await _loadDevicesByChild(
                              _selectedChild!.childId);
                        },
                      ),
                      onTap: () async {
                        await Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) => DeviceDetailChildScreen(device: device),
                          ),
                        );
                        await _loadDevicesByChild(_selectedChild!.childId);
                      },
                    ),
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}
