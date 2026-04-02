import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';

class ZoneSelectMapScreen extends StatefulWidget {

  const ZoneSelectMapScreen({super.key});

  @override
  State<ZoneSelectMapScreen> createState() => _ZoneSelectMapScreenState();
}

class _ZoneSelectMapScreenState extends State<ZoneSelectMapScreen> {
  LatLng? _selectedPoint;
  final LatLng _initialCenter = LatLng(
    10.850503,
    106.754150,
  ); // trung tâm mặc định

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Chọn vị trí vùng")),
      body: FlutterMap(
        options: MapOptions(
          initialCenter: _initialCenter, // thay center bằng initialCenter
          initialZoom: 16, // thay zoom bằng initialZoom
          onTap: (tapPosition, point) {
            setState(() => _selectedPoint = point);
          },
        ),
        children: [
          TileLayer(
            urlTemplate: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
            subdomains: const ['a', 'b', 'c'],
          ),
          if (_selectedPoint != null)
            MarkerLayer(
              markers: [
                Marker(
                  point: _selectedPoint!,
                  width: 40,
                  height: 40,
                  child: const Icon(
                    Icons.location_on,
                    color: Colors.orange,
                    size: 36,
                  ),
                ),
              ],
            ),
        ],
      ),
      floatingActionButton: _selectedPoint == null
    ? null
    : FloatingActionButton(
        child: const Icon(Icons.check),
        onPressed: () {
          Navigator.pop(context, _selectedPoint); // trả về LatLng
        },
      ),
    );
  }
}
