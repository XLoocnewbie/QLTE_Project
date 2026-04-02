import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';
import 'package:frontend_qlte/services/sos_request_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SOSRequestParentScreen extends StatefulWidget {
  final Map<String, dynamic> sosData;

  const SOSRequestParentScreen({super.key, required this.sosData});

  @override
  State<SOSRequestParentScreen> createState() => _SOSRequestParentScreenState();
}

class _SOSRequestParentScreenState extends State<SOSRequestParentScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  bool _isProcessing = false;
  bool _isHandled = false;

  late double viDo;
  late double kinhDo;
  late String sosId;
  late String thoiGian;
  late String trangThai;

  @override
  void initState() {
    super.initState();

    final data = widget.sosData;
    sosId = data['sosId'] ?? data['id'] ?? "Unknown";
    viDo = (data['viDo'] ?? 0).toDouble();
    kinhDo = (data['kinhDo'] ?? 0).toDouble();
    thoiGian = data['thoiGian'] ?? "Chưa rõ";
    trangThai = data['trangThai'] ?? "Đang xử lý";

    _animationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800),
    )..repeat(reverse: true);
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  /// 🔵 Gọi API cập nhật trạng thái SOS
  Future<void> _handleSOS() async {
    setState(() {
      _isProcessing = true;
    });

    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString("jwt_token") ?? "";

      final result = await SOSRequestService.updateSOSRequest(
        sosId: sosId,
        trangThai: "Đã xử lý",
        token: token,
      );

      if (result["success"] == true) {
        setState(() {
          _isHandled = true;
          trangThai = "Đã xử lý";
        });

        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text("✅ Đã cập nhật trạng thái SOS thành công."),
            backgroundColor: Colors.green,
          ),
        );
      } else {
        throw Exception(result["message"]);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("❌ Lỗi khi cập nhật SOS: $e"),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isProcessing = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final LatLng sosPosition = LatLng(viDo, kinhDo);

    return Scaffold(
      appBar: AppBar(
        title: const Text("🚨 Cảnh Báo SOS"),
        backgroundColor: Colors.redAccent,
      ),
      body: AnimatedBuilder(
        animation: _animationController,
        builder: (context, child) {
          final bgColor = Color.lerp(
            Colors.white,
            Colors.red.withOpacity(0.2),
            _animationController.value,
          );

          return Container(
            color: bgColor,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // 🗺️ Bản đồ OpenStreetMap thay cho Google Map
                Expanded(
                  flex: 2,
                  child: FlutterMap(
                    options: MapOptions(
                      initialCenter: sosPosition,
                      initialZoom: 16,
                    ),
                    children: [
                      TileLayer(
                        urlTemplate:
                        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                        subdomains: const ['a', 'b', 'c'],
                      ),
                      MarkerLayer(
                        markers: [
                          Marker(
                            point: sosPosition,
                            width: 50,
                            height: 50,
                            child: const Icon(
                              Icons.location_on,
                              color: Colors.red,
                              size: 40,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),

                // ℹ️ Thông tin SOS
                Expanded(
                  flex: 1,
                  child: Container(
                    padding: const EdgeInsets.all(20),
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.only(
                        topLeft: Radius.circular(24),
                        topRight: Radius.circular(24),
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black12,
                          blurRadius: 8,
                          offset: Offset(0, -3),
                        ),
                      ],
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          "🆔 SOS ID: $sosId",
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          "🕒 Thời gian: $thoiGian",
                          style: const TextStyle(fontSize: 16),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          "📍 Tọa độ: ($viDo, $kinhDo)",
                          style: const TextStyle(fontSize: 16),
                        ),
                        const SizedBox(height: 8),
                        Row(
                          children: [
                            const Text(
                              "⚙️ Trạng thái: ",
                              style: TextStyle(fontSize: 16),
                            ),
                            Text(
                              trangThai,
                              style: TextStyle(
                                fontSize: 16,
                                color: trangThai == "Đã xử lý"
                                    ? Colors.green
                                    : Colors.red,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                        const Spacer(),
                        Center(
                          child: ElevatedButton.icon(
                            onPressed:
                            _isProcessing || _isHandled ? null : _handleSOS,
                            icon: const Icon(Icons.check_circle_outline),
                            label: Text(
                              _isHandled
                                  ? "Đã xử lý xong"
                                  : "Xác nhận đã xử lý",
                              style: const TextStyle(fontSize: 16),
                            ),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: _isHandled
                                  ? Colors.grey
                                  : Colors.redAccent,
                              padding: const EdgeInsets.symmetric(
                                horizontal: 32,
                                vertical: 14,
                              ),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
