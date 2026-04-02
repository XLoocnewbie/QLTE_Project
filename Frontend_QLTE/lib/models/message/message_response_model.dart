class MessageResponseModel {
  final bool status;
  final String msg;
  final List<MessageDataModel>? data;

  MessageResponseModel({
    required this.status,
    required this.msg,
    required this.data,
  });

  factory MessageResponseModel.fromJson(Map<String, dynamic> json) {
    return MessageResponseModel(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: json['data'] != null
          ? (json['data'] as List).map((x) => MessageDataModel.fromJson(x)).toList()
          : [],
    );
  }
}

class MessageDataModel {
  final String messageId;
  final String senderId;
  final String receiverId;
  final String noiDung;
  final String loaiTinNhan;
  final DateTime thoiGian;
  final bool daDoc;

  // Thêm để hiển thị thông tin người gửi (nếu cần)
  String? senderName;
  String? senderAvatar;

  MessageDataModel({
    required this.messageId,
    required this.senderId,
    required this.receiverId,
    required this.noiDung,
    required this.loaiTinNhan,
    required this.thoiGian,
    required this.daDoc,
    this.senderName,
    this.senderAvatar,
  });

  factory MessageDataModel.fromJson(Map<String, dynamic> json) {
    DateTime parsedTime;
    if (json['thoiGian'] is int) {
      parsedTime = DateTime.fromMillisecondsSinceEpoch(json['thoiGian']);
    } else {
      parsedTime = DateTime.tryParse(json['thoiGian'] ?? '') ?? DateTime.now();
    }

    return MessageDataModel(
      messageId: json['messageId'] ?? '',
      senderId: json['senderId'] ?? '',
      receiverId: json['receiverId'] ?? '',
      noiDung: json['noiDung'] ?? '',
      loaiTinNhan: json['loaiTinNhan'] ?? 'Text',
      thoiGian: parsedTime,
      daDoc: json['daDoc'] ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'messageId': messageId,
      'senderId': senderId,
      'receiverId': receiverId,
      'noiDung': noiDung,
      'loaiTinNhan': loaiTinNhan,
      'thoiGian': thoiGian.toIso8601String(),
      'daDoc': daDoc,
    };
  }
}
