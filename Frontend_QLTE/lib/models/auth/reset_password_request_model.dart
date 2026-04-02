class ResetPasswordRequestModel {
  final String email;
  final String otp;
  final String type;
  final String newPassword;
  final String confirmNewPassword;

  ResetPasswordRequestModel({
    required this.email,
    required this.otp,
    required this.type,
    required this.newPassword,
    required this.confirmNewPassword,
  });

  factory ResetPasswordRequestModel.fromJson(Map<String, dynamic> json) {
    return ResetPasswordRequestModel(
      email: json['email'],
      otp: json['otp'],
      type: json['type'],
      newPassword: json['newPassword'],
      confirmNewPassword: json['confirmNewPassword'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'email': email,
      'otp': otp,
      'type': type,
      'newPassword': newPassword,
      'confirmNewPassword': confirmNewPassword,
    };
  }
}
