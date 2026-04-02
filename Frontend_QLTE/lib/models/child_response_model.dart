class ChildrenResponse {
  final bool status;
  final String msg;
  final List<Child> data;

  ChildrenResponse({
    required this.status,
    required this.msg,
    required this.data,
  });

  factory ChildrenResponse.fromJson(dynamic json) {
    List<Child> list = [];

    // kiểm tra json['data'] có phải list không
    if (json['data'] != null && json['data'] is List) {
      list = (json['data'] as List)
          .map((childJson) => Child.fromJson(childJson))
          .toList();
    }

    return ChildrenResponse(
      status: json['status'] ?? false,
      msg: json['msg']?.toString() ?? '',
      data: list,
    );
  }
}

class Child {
  final String childId;
  final String hoTen;
  final DateTime ngaySinh;
  final String gioiTinh;
  final String anhDaiDien;
  final String nhomTuoi;
  final String trangThai;
  final String userId;
  final String? userName;
  final String? phoneNumber;

  Child({
    required this.childId,
    required this.hoTen,
    required this.ngaySinh,
    required this.gioiTinh,
    required this.anhDaiDien,
    required this.nhomTuoi,
    required this.trangThai,
    required this.userId,
    this.userName,
    this.phoneNumber,
  });

  factory Child.fromJson(dynamic json) {
    return Child(
      childId: json['childId']?.toString() ?? '',
      hoTen: json['hoTen']?.toString() ?? '',
      ngaySinh: DateTime.tryParse(json['ngaySinh']?.toString() ?? '') ?? DateTime(2000),
      gioiTinh: json['gioiTinh']?.toString() ?? '',
      anhDaiDien: json['anhDaiDien']?.toString() ?? '',
      nhomTuoi: json['nhomTuoi']?.toString() ?? '',
      trangThai: json['trangThai']?.toString() ?? '',
      userId: json['userId']?.toString() ?? '',
      userName: json['userName']?.toString(),
      phoneNumber: json['phoneNumber']?.toString() ?? '',
    );
  }
}
