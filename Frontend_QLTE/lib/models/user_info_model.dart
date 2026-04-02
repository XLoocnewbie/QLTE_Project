class UserInfoModel {
  final String id;
  final String? userName;
  final String? nameND;
  final String? email;
  final String? phoneNumber;
  final int? gioiTinh;
  final String? moTa;
  final String? avatarND;
  final String? authId;
  final String typeLogin;
  final DateTime? thoiGianTao;
  final DateTime? thoiGianCapNhat;
  final DateTime? thoiGianDoiEmail;
  final List<String> roles;

  UserInfoModel({
    required this.id,
    this.userName,
    this.nameND,
    this.email,
    this.phoneNumber,
    this.gioiTinh,
    this.moTa,
    this.avatarND,
    this.authId,
    this.typeLogin = "Local",
    this.thoiGianTao,
    this.thoiGianCapNhat,
    this.thoiGianDoiEmail,
    this.roles = const [],
  });

  factory UserInfoModel.fromJson(Map<String, dynamic> json) {
    return UserInfoModel(
      id: json["id"] ?? "",
      userName: json["userName"],
      nameND: json["nameND"],
      email: json["email"],
      phoneNumber: json["phoneNumber"],
      gioiTinh: json["gioiTinh"],
      moTa: json["moTa"] ?? json["mota"],
      avatarND: json["avatarND"],
      authId: json["authId"],
      typeLogin: json["typeLogin"] ?? "Local",
      thoiGianTao: json["thoiGianTao"] != null
          ? DateTime.tryParse(json["thoiGianTao"])
          : null,
      thoiGianCapNhat: json["thoiGianCapNhat"] != null
          ? DateTime.tryParse(json["thoiGianCapNhat"])
          : null,
      thoiGianDoiEmail: json["thoiGianDoiEmail"] != null
          ? DateTime.tryParse(json["thoiGianDoiEmail"])
          : null,
      roles: json["roles"] != null
          ? List<String>.from(json["roles"])
          : [],
    );
  }
}
