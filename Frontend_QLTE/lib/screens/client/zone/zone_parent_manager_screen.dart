import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/zone/create_danger_zone_request_model.dart';
import 'package:frontend_qlte/models/zone/create_safe_zone_request_model.dart';
import 'package:frontend_qlte/models/zone/update_danger_zone_request_model.dart';
import 'package:frontend_qlte/models/zone/update_safe_zone_request_model.dart';
import 'package:frontend_qlte/screens/client/zone/zone_select_map_screent.dart';
import 'package:latlong2/latlong.dart';
import 'package:frontend_qlte/services/safe_zone_service.dart';
import 'package:frontend_qlte/services/danger_zone_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ZoneParentManagementScreen extends StatefulWidget {
  final String childId;
  final bool isSafeZone; // true = SafeZone, false = DangerZone

  const ZoneParentManagementScreen({
    super.key,
    required this.childId,
    required this.isSafeZone,
  });

  @override
  State<ZoneParentManagementScreen> createState() =>
      _ZoneParentManagementScreenState();
}

class _ZoneParentManagementScreenState
    extends State<ZoneParentManagementScreen> {
  bool _loading = true;
  List<dynamic> _zones = [];

  @override
  void initState() {
    super.initState();
    _loadZones();
  }

  Future<void> _loadZones() async {
    setState(() {
      _loading = true;
    });
    final token = await _getToken();
    var userId = JwtHelper.getUserId(token);

    if (widget.isSafeZone) {
      final res = await SafeZoneService.getSafeZoneByUserIdAndChildId(
        userId!,
        widget.childId,
      );
      if (res.status) _zones = res.data;
    } else {
      final res = await DangerZoneService.getDangerZoneByUserIdAndChildId(
        userId!,
        widget.childId,
      );
      if (res.status) _zones = res.data;
    }

    setState(() {
      _loading = false;
    });
  }

  Future<String> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString("jwt_token") ?? "";
  }

  void _deleteZone(dynamic zone) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Xác nhận xóa"),
        content: const Text("Bạn có chắc chắn muốn xóa vùng này không?"),
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

    if (confirm != true) return; // Người dùng bấm hủy
    bool success = false;

    if (widget.isSafeZone) {
      final res = await SafeZoneService.deleteSafeZone(zone.safeZoneId);
      success = res.status;
    } else {
      final res = await DangerZoneService.deleteDangerZone(
        zone.dangerZoneId,
      );
      success = res.status;
    }

    if (success) {
      setState(() {
        _zones.remove(zone);
      });
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Xóa vùng thành công")));
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Xóa vùng thất bại")));
    }
  }

  Future<void> _updateZone(
    dynamic zone,
    double lat,
    double lng,
    double radius,
    String name,
    String mota,
  ) async {
    bool success = false;

    if (widget.isSafeZone) {
      final request = UpdateSafeZoneRequest(
        safeZoneId: zone.safeZoneId,
        tenZone: name,
        viDo: lat,
        kinhDo: lng,
        banKinh: radius,
      );

      final res = await SafeZoneService.updateSafeZone(request);
      success = res.status;
    } else {
      final request = UpdateDangerZoneRequest(
        dangerZoneId: zone.dangerZoneId,
        tenKhuVuc: name,
        viDo: lat,
        kinhDo: lng,
        banKinh: radius,
        moTa: mota,
      );

      final res = await DangerZoneService.updateDangerZone(request);
      success = res.status;
    }
    if (success) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Cập nhật vùng thành công")));
      _loadZones();
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Cập nhật vùng thất bại")));
    }
  }

  void _showCreateZoneDialog(LatLng point) {
    final _nameController = TextEditingController();
    final _radiusController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text(
            widget.isSafeZone ? "Tạo vùng an toàn" : "Tạo vùng nguy hiểm",
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _nameController,
                decoration: InputDecoration(
                  labelText: widget.isSafeZone ? "Tên vùng" : "Tên khu vực",
                ),
              ),
              TextField(
                controller: _radiusController,
                decoration: const InputDecoration(labelText: "Bán kính (m)"),
                keyboardType: TextInputType.number,
              ),
              const SizedBox(height: 10),
              Text(
                "Vị trí: ${point.latitude.toStringAsFixed(5)}, ${point.longitude.toStringAsFixed(5)}",
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Hủy"),
            ),
            ElevatedButton(
              onPressed: () async {
                final name = _nameController.text.trim();
                final radius =
                    double.tryParse(_radiusController.text.trim()) ?? 0;
                if (name.isEmpty || radius <= 0) return;

                await _createZone(
                  point.latitude,
                  point.longitude,
                  radius,
                  name,
                );
                Navigator.pop(context);
              },
              child: const Text("Tạo"),
            ),
          ],
        );
      },
    );
  }

  void _showUpdateZoneDialog(dynamic zone, LatLng point) {
    final _nameController = TextEditingController(
      text: widget.isSafeZone ? zone.tenZone : zone.tenKhuVuc,
    );
    final _radiusController = TextEditingController(
      text: zone.banKinh.toString(),
    );
    final _descriptionController = TextEditingController(
      text: widget.isSafeZone ? "" : zone.moTa ?? "",
    );

    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text(
            widget.isSafeZone
                ? "Cập nhật vùng an toàn"
                : "Cập nhật vùng nguy hiểm",
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: _nameController,
                decoration: InputDecoration(
                  labelText: widget.isSafeZone ? "Tên vùng" : "Tên khu vực",
                ),
              ),
              TextField(
                controller: _radiusController,
                decoration: const InputDecoration(labelText: "Bán kính (m)"),
                keyboardType: TextInputType.number,
              ),
              if (!widget.isSafeZone)
                TextField(
                  controller: _descriptionController,
                  decoration: const InputDecoration(labelText: "Mô tả"),
                ),
              const SizedBox(height: 10),
              Text(
                "Vị trí mới: ${point.latitude.toStringAsFixed(5)}, ${point.longitude.toStringAsFixed(5)}",
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text("Hủy"),
            ),
            ElevatedButton(
              onPressed: () async {
                final name = _nameController.text.trim();
                final radius =
                    double.tryParse(_radiusController.text.trim()) ?? 0;
                final mota = _descriptionController.text.trim();

                if (name.isEmpty || radius <= 0) return;

                await _updateZone(
                  zone,
                  point.latitude,
                  point.longitude,
                  radius,
                  name,
                  mota,
                );
                Navigator.pop(context);
              },
              child: const Text("Lưu"),
            ),
          ],
        );
      },
    );
  }

  Future<void> _createZone(
    double lat,
    double lng,
    double radius,
    String name,
  ) async {
    final token = await _getToken();
    var userId = JwtHelper.getUserId(token);
    bool success = false;

    if (widget.isSafeZone) {
      final request = CreateSafeZoneRequest(
        userId: userId!,
        childrenId: widget.childId,
        tenZone: name,
        viDo: lat,
        kinhDo: lng,
        banKinh: radius,
      );
      final res = await SafeZoneService.createSafeZone(request);
      success = res.status;
    } else {
      final request = CreateDangerZoneRequest(
        userId: userId!,
        childrenId: widget.childId,
        tenKhuVuc: name,
        mota: "Nguy hiểm",
        viDo: lat,
        kinhDo: lng,
        banKinh: radius,
      );
      final res = await DangerZoneService.createDangerZone(request);
      success = res.status;
    }

    if (success) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Tạo vùng thành công")));
      _loadZones(); // load lại danh sách
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Tạo vùng thất bại")));
    }
  }

  Future<void> _openMapForUpdate(dynamic zone) async {
    final LatLng? selectedPoint = await Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => const ZoneSelectMapScreen()),
    );

    if (selectedPoint != null) {
      _showUpdateZoneDialog(zone, selectedPoint);
    }
  }

  void _openMapToSelectZone() async {
    final LatLng? selectedPoint = await Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => ZoneSelectMapScreen()),
    );

    if (selectedPoint != null) {
      _showCreateZoneDialog(selectedPoint);
    }
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: false, // chặn pop tự động
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          Navigator.pop(context, true); // quay lại và trả về true
        }
      },
      child: Scaffold(
        appBar: AppBar(
          title: Text(widget.isSafeZone ? "Vùng an toàn" : "Vùng nguy hiểm"),
        ),
        body: _loading
            ? const Center(child: CircularProgressIndicator())
            : ListView.builder(
                itemCount: _zones.length,
                itemBuilder: (context, index) {
                  final zone = _zones[index];
                  final name = widget.isSafeZone
                      ? zone.tenZone
                      : zone.tenKhuVuc;
                  return ListTile(
                    title: Text(name),
                    subtitle: Text("Radius: ${zone.banKinh}m"),
                    trailing: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        IconButton(
                          icon: const Icon(Icons.edit, color: Colors.blue),
                          onPressed: () => _openMapForUpdate(zone),
                        ),
                        IconButton(
                          icon: const Icon(Icons.delete, color: Colors.red),
                          onPressed: () => _deleteZone(zone),
                        ),
                      ],
                    ),
                  );
                },
              ),
        floatingActionButton: FloatingActionButton(
          child: const Icon(Icons.add),
          onPressed: _openMapToSelectZone,
        ),
      ),
    );
  }
}
