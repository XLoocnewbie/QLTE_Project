class ForgotPasswordRequestModel {
  final String email;
  final String type;

  ForgotPasswordRequestModel({
    required this.email,
    required this.type,
  });

  // Chuyển từ JSON sang object
  factory ForgotPasswordRequestModel.fromJson(Map<String, dynamic> json) {
    return ForgotPasswordRequestModel(
      email: json['email'] ?? '',
      type: json['type'] ?? '',
    );
  }

  // Chuyển từ object sang JSON
  Map<String, dynamic> toJson() {
    return {
      'email': email,
      'type': type,
    };
  }
}
