import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:frontend_qlte/models/zone/safe_zone_response_model.dart';
import 'package:frontend_qlte/screens/client/zone/zone_parent_manager_screen.dart';
import 'package:frontend_qlte/services/location_service.dart';
import 'package:frontend_qlte/services/safe_zone_service.dart';
import 'package:frontend_qlte/services/danger_zone_service.dart';
import 'package:frontend_qlte/services/signalr_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:latlong2/latlong.dart';
import 'package:shared_preferences/shared_preferences.dart';

class MapTrackingScreen extends StatefulWidget {
  final String childId;

  const MapTrackingScreen({super.key, required this.childId});

  @override
  State<MapTrackingScreen> createState() => _MapTrackingScreenState();
}

class _MapTrackingScreenState extends State<MapTrackingScreen> {
  LatLng? _childPos;
  List<SafeZone> _safeZones = [];
  List<dynamic> _dangerZones = [];
  bool _loading = true;

  final Distance _distance = Distance();
  final SignalRService _signalRService = SignalRService();

  @override
  void initState() {
    super.initState();
    _loadZonesAndLocation();
    _initSignalR();
  }

  Future<String> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString("jwt_token") ?? "";
  }

  Future<void> _loadZonesAndLocation() async {
    setState(() => _loading = true);
    final token = await _getToken();
    final userId = JwtHelper.getUserId(token);

    // Load SafeZones
    final safeRes = await SafeZoneService.getSafeZoneByUserIdAndChildId(
      userId!,
      widget.childId,
    );
    if (safeRes.status) _safeZones = safeRes.data;

    // Load DangerZones
    final dangerRes = await DangerZoneService.getDangerZoneByUserIdAndChildId(
      userId,
      widget.childId,
    );
    if (dangerRes.status) _dangerZones = dangerRes.data;

    // Load vị trí mới nhất
    final locRes = await LocationService.getLocationHistoryNewByChildId(
      widget.childId,
    );
    if (locRes.status && locRes.data.isNotEmpty) {
      final latest = locRes.data.first;
      _childPos = LatLng(latest.viDo, latest.kinhDo);
    }

    setState(() => _loading = false);
  }

  Future<void> _initSignalR() async {
    try {
      await _signalRService.connect();
      await _signalRService.joinChildGroup(widget.childId);
      _signalRService.onReceiveLocation((childId, lat, lng) {
        if (childId == widget.childId) {
          setState(() => _childPos = LatLng(lat, lng));
        }
      });
    } catch (e) {
      print("⚠️ Lỗi SignalR: $e");
    }
  }

  String _getChildStatus() {
    if (_childPos == null) return "⏳ Đang chờ tín hiệu từ thiết bị của bé...";

    for (var zone in _safeZones) {
      final center = LatLng(zone.viDo, zone.kinhDo);
      if (_distance(_childPos!, center) <= zone.banKinh) {
        return "📗 Bé đang ở vùng an toàn: ${zone.tenZone}";
      }
    }

    for (var zone in _dangerZones) {
      final center = LatLng(zone.viDo, zone.kinhDo);
      if (_distance(_childPos!, center) <= zone.banKinh) {
        return "📕 Bé đang ở vùng nguy hiểm: ${zone.tenKhuVuc}";
      }
    }

    return "🟡 Bé đang ở vùng cần lưu ý";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Theo dõi vị trí con")),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Stack(
              children: [
                FlutterMap(
                  options: MapOptions(
                    initialCenter: _childPos ?? LatLng(10.850503, 106.754150),
                    initialZoom: 16,
                  ),
                  children: [
                    TileLayer(
                      urlTemplate:
                          "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                      subdomains: const ['a', 'b', 'c'],
                    ),
                    // Vẽ SafeZones
                    for (var zone in _safeZones)
                      CircleLayer(
                        circles: [
                          CircleMarker(
                            point: LatLng(zone.viDo, zone.kinhDo),
                            useRadiusInMeter: true,
                            radius: zone.banKinh,
                            color: Colors.green.withOpacity(0.3),
                            borderColor: Colors.green,
                            borderStrokeWidth: 2,
                          ),
                        ],
                      ),
                    // Vẽ DangerZones
                    for (var zone in _dangerZones)
                      CircleLayer(
                        circles: [
                          CircleMarker(
                            point: LatLng(zone.viDo, zone.kinhDo),
                            useRadiusInMeter: true,
                            radius: zone.banKinh,
                            color: Colors.red.withOpacity(0.3),
                            borderColor: Colors.red,
                            borderStrokeWidth: 2,
                          ),
                        ],
                      ),
                    MarkerLayer(
                      markers: [
                        if (_childPos != null)
                          Marker(
                            point: _childPos!,
                            width: 40,
                            height: 40,
                            child: const Icon(
                              Icons.person_pin_circle,
                              color: Colors.blue,
                              size: 40,
                            ),
                          ),
                      ],
                    ),
                  ],
                ),
                Positioned(
                  top: 20,
                  left: 20,
                  right: 20,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    children: [
                      ElevatedButton.icon(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.green,
                        ),
                        onPressed: () async {
                          final result = await Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => ZoneParentManagementScreen(
                                childId: widget.childId,
                                isSafeZone: true,
                              ),
                            ),
                          );

                          if (result == true) {
                            // Nếu màn hình quản lý vùng trả về true thì reload lại bản đồ
                            await _loadZonesAndLocation();
                          }
                        },
                        icon: const Icon(Icons.shield),
                        label: const Text("Vùng an toàn"),
                      ),
                      ElevatedButton.icon(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.red,
                        ),
                        onPressed: () async {
                          final result = await Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => ZoneParentManagementScreen(
                                childId: widget.childId,
                                isSafeZone: false,
                              ),
                            ),
                          );

                          if (result == true) {
                            // Reload lại bản đồ khi có thay đổi vùng nguy hiểm
                            await _loadZonesAndLocation();
                          }
                        },
                        icon: const Icon(Icons.warning),
                        label: const Text("Vùng nguy hiểm"),
                      ),
                    ],
                  ),
                ),
                Positioned(
                  bottom: 20,
                  left: 20,
                  right: 20,
                  child: Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: Colors.black.withOpacity(0.6),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      _getChildStatus(),
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: Colors.white, fontSize: 16),
                    ),
                  ),
                ),
              ],
            ),
    );
  }
}
