import 'package:jwt_decoder/jwt_decoder.dart';

class JwtHelper {
  /// Giải mã token và trả về toàn bộ map claims
  static Map<String, dynamic> decodeToken(String token) {
    return JwtDecoder.decode(token);
  }

  /// Kiểm tra token có hết hạn không
  static bool isTokenExpired(String token) {
    return JwtDecoder.isExpired(token);
  }

  /// Lấy Role (vai trò)
  static String getRole(String token) {
    final decoded = JwtDecoder.decode(token);
    return decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  }

  /// Lấy UserId
  static String? getUserId(String token) {
    final decoded = JwtDecoder.decode(token);
    return decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
  }
  /// Lấy UserName
  static String? getUserName(String token) {
    final decoded = JwtDecoder.decode(token);
    return decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
  }

  /// Lấy Email
  static String? getEmail(String token) {
    final decoded = JwtDecoder.decode(token);
    return decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
  }

  /// Lấy Full Name (nếu có)
  static String? getFullName(String token) {
    final decoded = JwtDecoder.decode(token);
    return decoded["FullName"];
  }
}