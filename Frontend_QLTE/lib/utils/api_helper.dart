import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:frontend_qlte/services/auth_service.dart';
import 'package:frontend_qlte/main.dart';

class ApiHelper {
  static Future<http.Response> sendRequest(
    String method,
    Uri uri, {
    Map<String, String>? headers,
    dynamic body,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    String? token = prefs.getString("jwt_token");
    String? refresh = prefs.getString("refresh_token");

    // Nếu token hết hạn → refresh
    if (token != null && JwtHelper.isTokenExpired(token)) {
      print("⚠️ Token hết hạn, đang refresh...");
      if (refresh == null || refresh.isEmpty) {
        throw Exception("Không có refresh token, vui lòng đăng nhập lại");
      }

      final refreshResult = await AuthService.RefreshToken(refresh);
      if (refreshResult.status && refreshResult.data != null) {
        token = refreshResult.data!.token;
        await prefs.setString("jwt_token", token);
        print("✅ Token mới đã được lưu");
      } else {
        await prefs.clear();
        navigatorKey.currentState?.pushNamedAndRemoveUntil(
          '/login',
          (route) => false,
        );
        throw Exception(refreshResult.msg);
      }
    }

    // header mặc định
    final defaultHeaders = {
      "Content-Type": "application/json",
      "Authorization": "Bearer $token",
    };
    if (headers != null) {
      defaultHeaders.addAll(headers);
    }

    http.Response response;

    // Gửi request
    switch (method.toUpperCase()) {
      case "POST":
        response = await http.post(uri, headers: defaultHeaders, body: body);
        break;
      case "PUT":
        response = await http.put(uri, headers: defaultHeaders, body: body);
        break;
      case "DELETE":
        response = await http.delete(uri, headers: defaultHeaders);
        break;
      default:
        response = await http.get(uri, headers: defaultHeaders);
        break;
    }

    // Nếu server trả về 401 → token hết hạn nhưng refresh chưa xử lý
    if (response.statusCode == 401 && refresh != null && refresh.isNotEmpty) {
      print("⚠️ Token có thể đã hết hạn, thử refresh lại...");
      final refreshResult = await AuthService.RefreshToken(refresh);
      if (refreshResult.status && refreshResult.data != null) {
        token = refreshResult.data!.token;
        await prefs.setString("jwt_token", token);

        final retryHeaders = {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        };
        // gọi lại request
        switch (method.toUpperCase()) {
          case "POST":
            response = await http.post(uri, headers: retryHeaders, body: body);
            break;
          case "PUT":
            response = await http.put(uri, headers: retryHeaders, body: body);
            break;
          case "DELETE":
            response = await http.delete(uri, headers: retryHeaders);
            break;
          default:
            response = await http.get(uri, headers: retryHeaders);
            break;
        }
      }
    }

    return response;
  }

  static Future<String?> getValidToken() async {
    final prefs = await SharedPreferences.getInstance();
    String? token = prefs.getString("jwt_token");
    String? refresh = prefs.getString("refresh_token");

    if (token != null && JwtHelper.isTokenExpired(token)) {
      print("⚠️ Token hết hạn, đang refresh...");
      if (refresh == null || refresh.isEmpty) return null;

      final refreshResult = await AuthService.RefreshToken(refresh);
      if (refreshResult.status && refreshResult.data != null) {
        token = refreshResult.data!.token;
        await prefs.setString("jwt_token", token);
        print("✅ Token mới đã được lưu");
      } else {
        await prefs.clear();
        navigatorKey.currentState?.pushNamedAndRemoveUntil(
          '/login',
          (route) => false,
        );
        throw Exception(refreshResult.msg);
      }
    }
    return token;
  }
}
