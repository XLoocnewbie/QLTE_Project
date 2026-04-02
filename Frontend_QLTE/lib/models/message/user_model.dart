class UserModel {
  final String id;
  final String fullName;
  final String? avatar;
  final String userName;

  UserModel({
    required this.id,
    required this.fullName,
    required this.avatar,
    required this.userName,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      id: json['id'] ?? '',
      fullName: json['fullName'] ?? '',
      avatar: json['avatar'],
      userName: json['userName'] ?? '',
    );
  }
}
