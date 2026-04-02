class UserResponseModel {
  final String userId;
  final String userName;
  final String email;
  final String phoneNumber;
  final String nameND;
  final int gioiTinh;
  final String? mota;
  final String avatarND;
  final String typeLogin;
  final String role;
  final DateTime thoiGianCapNhat;
  final DateTime thoiGianTao;

  UserResponseModel({
    required this.userId,
    required this.userName,
    required this.email,
    required this.phoneNumber,
    required this.nameND,
    required this.gioiTinh,
    this.mota,
    required this.avatarND,
    required this.typeLogin,
    required this.role,
    required this.thoiGianCapNhat,
    required this.thoiGianTao,
  });

  factory UserResponseModel.fromJson(Map<String, dynamic> json) {
  return UserResponseModel(
    userId: json['userId'] ?? '',
    userName: json['userName'] ?? '',
    email: json['email'] ?? '',
    phoneNumber: json['phoneNumber'] ?? '',
    nameND: json['nameND'] ?? '',
    gioiTinh: json['gioiTinh'] ?? 0,
    mota: json['mota'] != null ? json['mota'] as String : null,
    avatarND: json['avatarND'] ?? '',
    typeLogin: json['typeLogin'] ?? '',
    role: json['role'] ?? '',
    thoiGianCapNhat: json['thoiGianCapNhat'] != null
        ? DateTime.parse(json['thoiGianCapNhat'])
        : DateTime.now(),
    thoiGianTao: json['thoiGianTao'] != null
        ? DateTime.parse(json['thoiGianTao'])
        : DateTime.now(),
  );
}

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'userName': userName,
      'email': email,
      'phoneNumber': phoneNumber,
      'nameND': nameND,
      'gioiTinh': gioiTinh,
      'mota': mota,
      'avatarND': avatarND,
      'typeLogin': typeLogin,
      'role': role,
      'thoiGianCapNhat': thoiGianCapNhat.toIso8601String(),
      'thoiGianTao': thoiGianTao.toIso8601String(),
    };
  }
}
