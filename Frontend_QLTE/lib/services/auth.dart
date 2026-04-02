import 'package:shared_preferences/shared_preferences.dart';
import 'auth_service.dart';

class Auth {
  static final AuthService _authService = AuthService();

  /// 🟢 Đăng nhập
  static Future<Map<String, dynamic>> login(String account, String password) async {
    var result = await _authService.login(account, password);
    return result;
  }

  /// 🟠 Đăng xuất
  static Future<bool> logout(String refreshToken) async {
    // Xoá token trong SharedPreferences
    final prefs = await SharedPreferences.getInstance();
    final logout = await AuthService.UserLogOut(refreshToken);
    if(logout.status){
      await prefs.remove('jwt_token');
      await prefs.remove('refresh_token');
      await prefs.remove('user_id');
      return true;
    }
    return false;
  }
}
