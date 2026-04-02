import 'package:flutter/material.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/child_response_model.dart';
import 'package:frontend_qlte/screens/client/map_tracking_screen.dart';
import 'package:frontend_qlte/services/child_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ChildrenListScreen extends StatefulWidget {
  const ChildrenListScreen({super.key});

  @override
  State<ChildrenListScreen> createState() => _ChildrenListScreenState();
}

class _ChildrenListScreenState extends State<ChildrenListScreen> {
  Future<ChildrenResponse>? _futureChildren;

  @override
  void initState() {
    super.initState();
    _loadChildren();
  }

  void _loadChildren() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString("jwt_token") ?? "";
    if (token.isEmpty) return;
    final userId = JwtHelper.getUserId(token);

    setState(() {
      _futureChildren = ChildService.getChildrenByUserId(userId!);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Danh sách Children"),automaticallyImplyLeading: false),
      body: _futureChildren == null
          ? const Center(child: CircularProgressIndicator())
          : FutureBuilder<ChildrenResponse>(
              future: _futureChildren,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                } else if (snapshot.hasError) {
                  return Center(child: Text("Lỗi: ${snapshot.error}"));
                } else if (!snapshot.hasData || snapshot.data!.data.isEmpty) {
                  return const Center(child: Text("Chưa có dữ liệu children"));
                }

                final children = snapshot.data!.data;
                
                return ListView.builder(
                  itemCount: children.length,
                  itemBuilder: (context, index) {
                    final child = children[index];
                    final inSafeZone = child.trangThai.toLowerCase() == "true";
                    return ListTile(
                      leading: CircleAvatar(
                        backgroundImage:
                            (child.anhDaiDien.isNotEmpty)
                            ? NetworkImage(
                                "${Config_URL.urlServer}${child.anhDaiDien}",
                              )
                            : null,
                        child:
                            (child.anhDaiDien.isEmpty)
                            ? Text(
                                child.hoTen.isNotEmpty ? child.hoTen : "?",
                              )
                            : null,
                      ),
                      title: Text(child.hoTen),
                      subtitle: Text(
                        inSafeZone
                            ? "✅ Trong vùng an toàn"
                            : "🚨 Ngoài vùng an toàn",
                      ),
                      trailing: const Icon(Icons.arrow_forward_ios, size: 18),
                      onTap: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) =>
                                MapTrackingScreen(childId: child.childId),
                          ),
                        );
                      },
                    );
                  },
                );
              },
            ),
    );
  }
}
