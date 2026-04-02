class UpdateUserInfoRequestModel {
  final String userId;
  final String nameND;
  final String userName;
  final String phoneNumber;
  final String? avatarND;
  final int? gioiTinh;
  final String? moTa;

  UpdateUserInfoRequestModel({
    required this.userId,
    required this.nameND,
    required this.userName,
    required this.phoneNumber,
    this.avatarND,
    this.gioiTinh,
    this.moTa,
  });

  Map<String, dynamic> toJson() {
    return {
      'UserId': userId,
      'NameND': nameND,
      'UserName': userName,
      'PhoneNumber': phoneNumber,
      'AvatarND': avatarND ?? '',
      'GioiTinh': gioiTinh ?? 0,
      'MoTa': moTa ?? '',
    };
  }

  factory UpdateUserInfoRequestModel.fromJson(Map<String, dynamic> json) {
    return UpdateUserInfoRequestModel(
      userId: json['UserId'] ?? '',
      nameND: json['NameND'] ?? '',
      userName: json['UserName'] ?? '',
      phoneNumber: json['PhoneNumber'] ?? '',
      avatarND: json['AvatarND'],
      gioiTinh: json['GioiTinh'],
      moTa: json['MoTa'],
    );
  }
}
