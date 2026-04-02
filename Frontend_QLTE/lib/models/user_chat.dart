class UserChat {
  final int adoptionRequestId;
  final String receiverId;
  final String receiverEmail;
  final String lastMessage;
  final DateTime lastTime;

  UserChat({
    required this.adoptionRequestId,
    required this.receiverId,
    required this.receiverEmail,
    required this.lastMessage,
    required this.lastTime,
  });

  factory UserChat.fromJson(Map<String, dynamic> json) {
    return UserChat(
      adoptionRequestId: json["adoptionRequestId"] ?? 0,
      receiverId: json["receiverId"] ?? "",
      receiverEmail: json["receiverEmail"] ?? "",
      lastMessage: json["lastMessage"] ?? "",
      lastTime: DateTime.tryParse(json["lastTime"] ?? "") ?? DateTime.now(),
    );
  }
}
