class ChatMessage {
  final int id;
  final int adoptionRequestId;
  final String senderId;
  final String receiverId;
  final String noiDung;
  final DateTime thoiGianGui;
  final bool daDoc;

  ChatMessage({
    required this.id,
    required this.adoptionRequestId,
    required this.senderId,
    required this.receiverId,
    required this.noiDung,
    required this.thoiGianGui,
    required this.daDoc,
  });

  factory ChatMessage.fromJson(Map<String, dynamic> json) {
    return ChatMessage(
      id: json["id"] ?? 0,
      adoptionRequestId: json["adoptionRequestId"] ?? 0,
      senderId: json["senderId"] ?? "",
      receiverId: json["receiverId"] ?? "",
      noiDung: json["noiDung"] ?? "",
      thoiGianGui: DateTime.tryParse(json["thoiGianGui"] ?? "") ?? DateTime.now(),
      daDoc: json["daDoc"] ?? false,
    );
  }
}
