import 'package:frontend_qlte/models/message/message_model.dart';

class MessagesModel {
  final bool status;
  final String msg;
  final List<MessageModel> data;

  MessagesModel({
    required this.status,
    required this.msg,
    required this.data,
  });

  factory MessagesModel.fromJson(Map<String, dynamic> json) {
    return MessagesModel(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: (json['data'] as List<dynamic>?)
              ?.map((e) => MessageModel.fromJson(e))
              .toList() ??
          [],
    );
  }
}