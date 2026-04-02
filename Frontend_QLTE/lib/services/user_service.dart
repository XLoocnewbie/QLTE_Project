import 'dart:convert';
import 'package:frontend_qlte/models/user/update_user_info_request_model.dart';
import 'package:frontend_qlte/models/user_response_model.dart';
import 'package:frontend_qlte/utils/api_helper.dart';
import 'package:http/http.dart' as http;
import 'package:frontend_qlte/config/config_url.dart';
import '../models/user_info_model.dart';

class UserService {
  static final String _baseUrl = "${Config_URL.baseUrl}User/";

  /// 🟢 Đăng ký người dùng
  static Future<Map<String, dynamic>> registerUser({
    required String email,
    required String password,
    required String nameND,
    int? gioiTinh,
    String? moTa,
  }) async {
    final url = Uri.parse("${_baseUrl}RegisterUser");

    final body = jsonEncode({
      "email": email,
      "password": password,
      "nameND": nameND,
      "gioiTinh": gioiTinh,
      "moTa": moTa,
    });

    final response = await http.post(
      url,
      headers: {"Content-Type": "application/json"},
      body: body,
    );

    final result = jsonDecode(response.body);
    return {
      "success": result["status"] ?? false,
      "message": result["msg"] ?? "Đăng ký thất bại",
    };
  }

  /// 🟢 Đăng nhập (Local)
  static Future<Map<String, dynamic>> loginLocal({
    required String account,
    required String password,
  }) async {
    final url = Uri.parse("${_baseUrl}LoginLocalUser");

    final response = await http.post(
      url,
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"account": account, "password": password}),
    );

    final result = jsonDecode(response.body);
    return {
      "success": result["status"] ?? false,
      "message": result["msg"] ?? "Đăng nhập thất bại",
      "data": result["data"],
    };
  }

  /// 🟢 Cập nhật thông tin người dùng
  static Future<Map<String, dynamic>> updateInfoUser({
    required String userId,   // 🟢 bắt buộc
    required String userName, // 🟢 thêm mới
    String? nameND,
    int? gioiTinh,
    String? moTa,
    String? phoneNumber,
    String? avatarPath,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}UpdateInfoUser");

    try {
      var request = http.MultipartRequest("PUT", url)
        ..headers["Authorization"] = "Bearer $token"
        ..fields["UserId"] = userId
        ..fields["UserName"] = userName; // ✅ BẮT BUỘC backend cần

      if (nameND != null && nameND.isNotEmpty) {
        request.fields["NameND"] = nameND;
      }
      if (gioiTinh != null) {
        request.fields["GioiTinh"] = gioiTinh.toString();
      }
      if (moTa != null && moTa.isNotEmpty) {
        request.fields["MoTa"] = moTa;
      }
      if (phoneNumber != null && phoneNumber.isNotEmpty) {
        request.fields["PhoneNumber"] = phoneNumber;
      }

      if (avatarPath != null && avatarPath.isNotEmpty) {
        request.files.add(
          await http.MultipartFile.fromPath("AvatarND", avatarPath),
        );
      }

      print("📤 Gửi request cập nhật:");
      print("  ➤ URL: $url");
      print("  ➤ Fields: ${request.fields}");

      var streamedResponse = await request.send();
      var response = await http.Response.fromStream(streamedResponse);

      print("📥 Response status: ${response.statusCode}");
      print("📥 Response body: ${response.body}");

      if (response.statusCode == 200) {
        final result = jsonDecode(response.body);
        return {
          "success": result["status"] ?? false,
          "message": result["msg"] ?? "Cập nhật thành công",
        };
      } else {
        throw Exception("Lỗi khi cập nhật: ${response.statusCode}");
      }
    } catch (e) {
      print("❌ Lỗi updateInfoUser: $e");
      return {"success": false, "message": e.toString()};
    }
  }

  /// 🟢 Lấy danh sách người dùng (Admin)
  static Future<List<UserResponseModel>> getAllUsers({
    int page = 1,
    int limit = 10,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}GetListUser?page=$page&limit=$limit");

    final response = await http.get(
      url,
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode == 200) {
      final Map<String, dynamic> jsonData = jsonDecode(response.body);

      if (jsonData["status"] == true && jsonData["data"] != null) {
        final List<dynamic> dataList = jsonData["data"];
        return dataList.map((e) => UserResponseModel.fromJson(e)).toList();
      } else {
        return [];
      }
    } else {
      throw Exception("Lỗi server: ${response.statusCode}");
    }
  }

  /// 🟢 Lấy thông tin User theo email
  static Future<UserInfoModel?> getUserByEmail(String email) async {
    final url = Uri.parse("${_baseUrl}GetUserByEmail?Email=$email");
    final response = await http.get(url);

    if (response.statusCode == 200) {
      final result = jsonDecode(response.body);
      if (result["status"] == true && result["data"] != null) {
        return UserInfoModel.fromJson(result["data"]);
      }
    }
    return null;
  }

  /// 🟢 Lấy thông tin User theo UserId
  static Future<UserResponseModel?> getUserByUserId(String userId) async {
    final url = Uri.parse("${_baseUrl}GetUserByUserId?UserId=$userId");
    final response = await http.get(url);

    if (response.statusCode == 200) {
      final result = jsonDecode(response.body);
      if (result["status"] == true && result["data"] != null) {
        return UserResponseModel.fromJson(result["data"]);
      }
    }
    return null;
  }

  /// 🟢 Xoá User (Admin)
  static Future<Map<String, dynamic>> deleteUser({
    required String userId,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}DeleteUser?UserId=$userId");
    final response = await http.delete(
      url,
      headers: {"Authorization": "Bearer $token"},
    );

    final result = jsonDecode(response.body);
    return {
      "success": result["status"] ?? false,
      "message": result["msg"] ?? "Không thể xoá người dùng",
    };
  }

  static Future<UserResponseModel?> updateInfoUserAsync(
  UpdateUserInfoRequestModel request,
) async {
  final uri = Uri.parse('${_baseUrl}UpdateInfoUser');

  try {
    final token = await ApiHelper.getValidToken();
    if (token == null) throw Exception("Token không hợp lệ");

    var multipart = http.MultipartRequest('PUT', uri);
    multipart.headers['Authorization'] = 'Bearer $token';
    multipart.headers['Accept'] = 'application/json';

    // Bắt buộc không được rỗng
    if (request.userId.isEmpty || request.nameND.isEmpty || request.userName.isEmpty) {
      throw Exception("UserId, NameND và UserName không được rỗng");
    }

    multipart.fields['UserId'] = request.userId;
    multipart.fields['NameND'] = request.nameND;
    multipart.fields['UserName'] = request.userName;
    multipart.fields['PhoneNumber'] = request.phoneNumber;
    multipart.fields['GioiTinh'] = (request.gioiTinh ?? 0).toString();
    multipart.fields['MoTa'] = request.moTa ?? '';

    // Nếu có ảnh mới attach, nếu không gửi rỗng
    if (request.avatarND != null && request.avatarND!.isNotEmpty) {
      multipart.files.add(
        await http.MultipartFile.fromPath('AvatarND', request.avatarND!),
      );
    } else {
      // gửi một file rỗng hoặc bỏ field tuỳ server
      multipart.fields['AvatarND'] = '';
    }

    final streamedResponse = await multipart.send();
    final response = await http.Response.fromStream(streamedResponse);

    if (response.statusCode == 200) {
      final jsonRes = jsonDecode(response.body);
      return UserResponseModel.fromJson(jsonRes["data"]);
    } else {
      print('❌ Lỗi server: ${response.statusCode} - ${response.body}');
      return null;
    }
  } catch (e) {
    print('❌ Exception updateInfoUserAsync: $e');
    return null;
  }
}


  static Future<UserResponseModel?> getUserByUserIdAsync(String userId) async {
    final url = Uri.parse("${_baseUrl}GetUserByUserId?UserId=$userId");

    try {
      final response = await ApiHelper.sendRequest("GET", url);

      if (response.statusCode == 200) {
        final result = jsonDecode(response.body);
        if (result["status"] == true && result["data"] != null) {
          return UserResponseModel.fromJson(result["data"]);
        }
      } else {
        print("⚠️ Lỗi khi lấy user: ${response.statusCode} - ${response.body}");
      }
    } catch (e) {
      print("❌ Exception khi lấy user: $e");
    }
  }
}
