class VerifyForgotPasswordRequestModel {
  final String email;
  final String otp;
  final String type;

  VerifyForgotPasswordRequestModel({
    required this.email,
    required this.otp,
    required this.type,
  });

  // Chuyển từ JSON sang object
  factory VerifyForgotPasswordRequestModel.fromJson(Map<String, dynamic> json) {
    return VerifyForgotPasswordRequestModel(
      email: json['email'] ?? '',
      otp: json['otp'] ?? '',
      type: json['type'] ?? '',
    );
  }

  // Chuyển từ object sang JSON
  Map<String, dynamic> toJson() {
    return {
      'email': email,
      'otp': otp,
      'type': type,
    };
  }
}
