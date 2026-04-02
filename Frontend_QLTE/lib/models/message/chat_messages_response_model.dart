
import 'package:frontend_qlte/models/message/messages_model.dart';


class ChatMessagesResponseModel {
  final int total;
  final int page;
  final int pageSize;
  final MessagesModel messages;

  ChatMessagesResponseModel({
    required this.total,
    required this.page,
    required this.pageSize,
    required this.messages,
  });

  factory ChatMessagesResponseModel.fromJson(Map<String, dynamic> json) {
    return ChatMessagesResponseModel(
      total: json['total'] ?? 0,
      page: json['page'] ?? 1,
      pageSize: json['pageSize'] ?? 20,
      messages: MessagesModel.fromJson(json['messages'] ?? {}),
    );
  }
}
