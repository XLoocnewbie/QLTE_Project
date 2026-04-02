import 'dart:convert';
import 'dart:io';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/child_response_model.dart';
import 'package:frontend_qlte/models/parent_with_children_model.dart';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/utils/api_helper.dart';

class ChildService {
  static final String _baseUrl = '${Config_URL.baseUrl}Child';

  static Future<ChildrenResponse> getChildrenByUserId(String userId) async {
    try {
      final uri = Uri.parse('$_baseUrl/GetChildrenByUserId?userId=$userId');

      // 👉 Gọi ApiHelper để tự động xử lý token & refresh nếu cần
      final response = await ApiHelper.sendRequest("GET", uri);

      if (response.statusCode == 200) {
        final jsonData = jsonDecode(response.body);
        return ChildrenResponse.fromJson(jsonData);
      } else {
        return ChildrenResponse(
          status: false,
          msg: 'Lỗi: ${response.statusCode}',
          data: [],
        );
      }
    } catch (e) {
      return ChildrenResponse(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// Lấy thông tin 1 Child (Children role)
  static Future<Map<String, dynamic>> getChildByUserId(
      String userId, String token) async {
    final url =
    Uri.parse("${Config_URL.baseUrl}Child/GetChildByUserId?userId=$userId");
    print("📡 [GET] $url");

    final response = await http.get(url, headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer $token",
    });

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    if (response.statusCode == 200) {
      final body = jsonDecode(response.body);
      if (body["status"] == true && body["data"] != null) {
        final child = Child.fromJson(body["data"]);
        return {"success": true, "data": child};
      } else {
        return {
          "success": false,
          "message": body["msg"] ?? "Không tìm thấy thông tin trẻ."
        };
      }
    } else {
      return {"success": false, "message": "Lỗi API: ${response.statusCode}"};
    }
  }

  /// Tạo mới Child (POST /CreateChild)
  static Future<Map<String, dynamic>> createChild(
      Map<String, String> fields,
      String token, {
        File? avatar,
      }) async {
    final url = Uri.parse("${Config_URL.baseUrl}Child/CreateChild");
    print("📡 [POST] $url");

    final request = http.MultipartRequest("POST", url);
    request.headers["Authorization"] = "Bearer $token";

    // Thêm các field text
    fields.forEach((key, value) {
      request.fields[key] = value;
    });

    // Thêm file AvatarND nếu có
    if (avatar != null) {
      print("📤 Upload avatar file: ${avatar.path}");
      request.files.add(await http.MultipartFile.fromPath("AvatarND", avatar.path));
    } else {
      print("⚠️ Không có avatar được chọn → backend sẽ báo lỗi nếu AvatarND required");
    }

    // Gửi request
    final response = await request.send();
    final responseBody = await response.stream.bytesToString();

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: $responseBody");

    if (response.statusCode == 200) {
      final body = jsonDecode(responseBody);
      return {
        "success": body["status"] == true,
        "message": body["msg"] ?? "Tạo trẻ thành công",
      };
    } else {
      return {
        "success": false,
        "message": "Lỗi API: ${response.statusCode}",
      };
    }
  }

  /// Cập nhật Child (PUT /UpdateChild)
  static Future<Map<String, dynamic>> updateChild(
      Map<String, String> fields, String token) async {
    final url = Uri.parse("${Config_URL.baseUrl}Child/UpdateChild");
    print("📡 [PUT] $url");

    final request = http.MultipartRequest("PUT", url);
    request.headers["Authorization"] = "Bearer $token";

    fields.forEach((key, value) => request.fields[key] = value);

    final response = await request.send();
    final responseBody = await response.stream.bytesToString();

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: $responseBody");

    if (response.statusCode == 200) {
      final body = jsonDecode(responseBody);
      return {
        "success": body["status"] == true,
        "message": body["msg"] ?? "Cập nhật thành công"
      };
    } else {
      return {"success": false, "message": "Lỗi API: ${response.statusCode}"};
    }
  }

  /// Xoá Child
  static Future<Map<String, dynamic>> deleteChild(
      String childId, String token) async {
    final url =
    Uri.parse("${Config_URL.baseUrl}Child/DeleteChild?childrenId=$childId");
    print("📡 [DELETE] $url");

    final response = await http.delete(url, headers: {
      "Authorization": "Bearer $token",
    });

    print("📥 Status: ${response.statusCode}");
    print("📥 Body: ${response.body}");

    if (response.statusCode == 200) {
      final body = jsonDecode(response.body);
      return {
        "success": body["status"] == true,
        "message": body["msg"] ?? "Xoá thành công"
      };
    } else {
      return {"success": false, "message": "Lỗi API: ${response.statusCode}"};
    }
  }

  /// 🟣 Dành riêng cho Admin: Lấy danh sách tất cả Parent và con của họ
  static Future<Map<String, dynamic>> getAllParentsWithChildren(String token) async {
    final url = Uri.parse("${Config_URL.baseUrl}Child/GetAllParentsWithChildren");
    print("📡 [GET] $url");

    try {
      final response = await http.get(
        url,
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );

      print("📥 Status: ${response.statusCode}");
      print("📥 Body: ${response.body}");

      if (response.statusCode == 200) {
        final body = jsonDecode(response.body);
        if (body["status"] == true && body["data"] != null) {
          // ✅ Parse dữ liệu Parent + Children
          final List<dynamic> list = body["data"];
          final parents = list
              .map((item) => ParentWithChildren.fromJson(item))
              .toList();

          return {"success": true, "data": parents};
        } else {
          return {"success": false, "message": body["msg"] ?? "Không có dữ liệu"};
        }
      } else {
        return {
          "success": false,
          "message": "Lỗi API: ${response.statusCode}"
        };
      }
    } catch (e) {
      print("❌ Lỗi kết nối: $e");
      return {
        "success": false,
        "message": "Không thể kết nối tới server"
      };
    }
  }
}
