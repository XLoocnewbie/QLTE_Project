import 'package:frontend_qlte/models/message/user_model.dart';

class MessageModel {
  final String messageID;
  final String content;
  final DateTime timestamp;
  final bool seen;
  final String type;
  final UserModel fromUser;
  final UserModel toUser;

  MessageModel({
    required this.messageID,
    required this.content,
    required this.timestamp,
    required this.seen,
    required this.type,
    required this.fromUser,
    required this.toUser,
  });

  // ✅ Hàm copyWith đúng kiểu dữ liệu
  MessageModel copyWith({
    String? messageID,
    String? content,
    DateTime? timestamp,
    bool? seen,
    String? type,
    UserModel? fromUser,
    UserModel? toUser,
  }) {
    return MessageModel(
      messageID: messageID ?? this.messageID,
      content: content ?? this.content,
      timestamp: timestamp ?? this.timestamp,
      seen: seen ?? this.seen,
      type: type ?? this.type,
      fromUser: fromUser ?? this.fromUser,
      toUser: toUser ?? this.toUser,
    );
  }

  factory MessageModel.fromJson(Map<String, dynamic> json) {
    return MessageModel(
      messageID: json['messageID'] ?? '',
      content: json['content'] ?? '',
      timestamp: DateTime.tryParse(json['timestamp'] ?? '') ?? DateTime.now(),
      seen: json['seen'] ?? false,
      type: json['type'] ?? 'Text',
      fromUser: UserModel.fromJson(json['fromUser'] ?? {}),
      toUser: UserModel.fromJson(json['toUser'] ?? {}),
    );
  }
}
