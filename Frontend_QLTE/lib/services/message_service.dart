import 'dart:convert';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/message/chat_messages_response_model.dart';
import 'package:frontend_qlte/models/message/message_response_model.dart';
import 'package:frontend_qlte/models/message/messages_model.dart';
import 'package:frontend_qlte/utils/api_helper.dart';

class MessageService {
  static final String _baseUrl = '${Config_URL.baseUrl}Message';

  /// 🔹 Lấy tin nhắn cuối cùng của mỗi user
  static Future<MessageResponseModel> getLastMessagePerUser(String userId) async {
    try {
      final uri = Uri.parse('$_baseUrl/GetLastMessagePerUser?UserId=$userId');

      // 👇 Dùng ApiHelper để gửi request
      final response = await ApiHelper.sendRequest('GET', uri);

      final jsonData = jsonDecode(response.body);
      if (response.statusCode == 200) {
        return MessageResponseModel.fromJson(jsonData);
      } else {
        return MessageResponseModel(
          status: false,
          msg: jsonData['msg'] ?? 'Lỗi: ${response.statusCode}',
          data: [],
        );
      }
    } catch (e) {
      return MessageResponseModel(
        status: false,
        msg: 'Không thể kết nối server: $e',
        data: [],
      );
    }
  }

  /// 🔹 Lấy lịch sử tin nhắn giữa 2 người
  static Future<ChatMessagesResponseModel> getHistory(
    String userId1,
    String userId2,
    int page,
    int pageSize,
  ) async {
    try {
      final uri = Uri.parse(
          '$_baseUrl/History?user1Id=$userId1&user2Id=$userId2&page=$page&pageSize=$pageSize');

      // 👇 Gọi qua ApiHelper
      final response = await ApiHelper.sendRequest('GET', uri);
      final jsonData = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return ChatMessagesResponseModel.fromJson(jsonData);
      } else {
        return ChatMessagesResponseModel(
          total: 0,
          page: page,
          pageSize: pageSize,
          messages: MessagesModel(
            status: false,
            msg: jsonData['msg'] ?? 'Lỗi: ${response.statusCode}',
            data: [],
          ),
        );
      }
    } catch (e) {
      return ChatMessagesResponseModel(
        total: 0,
        page: page,
        pageSize: pageSize,
        messages: MessagesModel(
          status: false,
          msg: 'Không thể kết nối server: $e',
          data: [],
        ),
      );
    }
  }
}