import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/config_url.dart';
import '../models/chat_message.dart';
import '../models/user_chat.dart';

class ChatMessageService {
  static final String _baseUrl = "${Config_URL.baseUrl}ChatMessage/";

  /// 🟢 1️⃣ Gửi tin nhắn mới
  static Future<Map<String, dynamic>> createChatMessage({
    required int adoptionRequestId,
    required int petId, // 🟢 Thêm tham số petId
    required String receiverId,
    required String noiDung,
    required String token,
  }) async {
    final url = Uri.parse("${_baseUrl}CreateChatMessage");

    // 🟢 Body JSON gửi kèm petId
    final body = jsonEncode({
      "adoptionRequestId": adoptionRequestId,
      "petId": petId, // 🟢 thêm vào đây
      "receiverId": receiverId,
      "noiDung": noiDung,
    });

    final response = await http.post(
      url,
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
      body: body,
    );

    final result = jsonDecode(response.body);
    if (response.statusCode == 200 && (result["status"] == true)) {
      return {
        "success": true,
        "message": result["msg"] ?? "Gửi tin nhắn thành công",
        "data": result["data"], // 🟢 để lấy adoptionRequestId mới nếu có
      };
    } else {
      return {
        "success": false,
        "message": result["msg"] ?? "Gửi tin nhắn thất bại",
      };
    }
  }

  /// 🟢 2️⃣ Lấy danh sách tin nhắn theo AdoptionRequestId
  static Future<List<ChatMessage>> getMessagesByAdoptionRequest({
    required int adoptionRequestId,
    int page = 1,
    int limit = 20,
    required String token,
  }) async {
    final url = Uri.parse(
        "${_baseUrl}GetChatMessagesByAdoptionRequest?adoptionRequestId=$adoptionRequestId&page=$page&limit=$limit");

    final response = await http.get(
      url,
      headers: {
        "Authorization": "Bearer $token",
      },
    );

    if (response.statusCode == 200) {
      final Map<String, dynamic> jsonData = jsonDecode(response.body);
      if (jsonData["status"] == true && jsonData["data"] != null) {
        final List<dynamic> list = jsonData["data"];
        return list.map((e) => ChatMessage.fromJson(e)).toList();
      } else {
        throw Exception(jsonData["msg"] ?? "Không có tin nhắn nào");
      }
    } else {
      throw Exception("Lỗi tải tin nhắn: ${response.statusCode}");
    }
  }

  /// 🟢 3️⃣ Lấy danh sách các cuộc trò chuyện
  static Future<List<UserChat>> getUserChats({required String token}) async {
    final url = Uri.parse("${_baseUrl}GetUserChats");

    final response = await http.get(
      url,
      headers: {
        "Authorization": "Bearer $token",
      },
    );

    if (response.statusCode == 200) {
      final Map<String, dynamic> jsonData = jsonDecode(response.body);
      if (jsonData["status"] == true && jsonData["data"] != null) {
        final List<dynamic> list = jsonData["data"];
        return list.map((e) => UserChat.fromJson(e)).toList();
      } else {
        throw Exception(jsonData["msg"] ?? "Không có cuộc trò chuyện nào");
      }
    } else {
      throw Exception(
          "Lỗi tải danh sách cuộc trò chuyện: ${response.statusCode}");
    }
  }
}
