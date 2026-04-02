// Model chính chứa danh sách DangerZone
// Model chính chứa danh sách DangerZone
class DangerZoneResponse {
  bool status;
  String msg;
  List<DangerZone> data;
  dynamic pagination; // null hoặc object

  DangerZoneResponse({
    required this.status,
    required this.msg,
    required this.data,
    this.pagination,
  });

  // Parse từ JSON
  factory DangerZoneResponse.fromJson(Map<String, dynamic> json) {
    var dataField = json['data'];
    List<DangerZone> parsedData = [];

    if (dataField != null) {
      if (dataField is List) {
        parsedData =
            List<DangerZone>.from(dataField.map((x) => DangerZone.fromJson(x)));
      } else if (dataField is Map<String, dynamic>) {
        parsedData = [DangerZone.fromJson(dataField)];
      }
    }

    return DangerZoneResponse(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: parsedData,
      pagination: json['pagination'],
    );
  }

  // Chuyển về JSON
  Map<String, dynamic> toJson() => {
        'status': status,
        'msg': msg,
        'data': List<dynamic>.from(data.map((x) => x.toJson())),
        'pagination': pagination,
      };
}


// Model cho từng DangerZone
class DangerZone {
  String dangerZoneId;
  String userId;
  String tenKhuVuc;
  double viDo;
  double kinhDo;
  double banKinh;
  String? moTa;
  String childrenId;

  DangerZone({
    required this.dangerZoneId,
    required this.userId,
    required this.tenKhuVuc,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
    this.moTa,
    required this.childrenId,
  });

  // Parse từ JSON
  factory DangerZone.fromJson(Map<String, dynamic> json) {
    return DangerZone(
      dangerZoneId: json['dangerZoneId'] ?? '',
      userId: json['userId'] ?? '',
      tenKhuVuc: json['tenKhuVuc'] ?? '',
      viDo: (json['viDo'] as num?)?.toDouble() ?? 0.0,
      kinhDo: (json['kinhDo'] as num?)?.toDouble() ?? 0.0,
      banKinh: (json['banKinh'] as num?)?.toDouble() ?? 0.0,
      moTa: json['moTa'],
      childrenId: json['childrenId'] ?? '',
    );
  }

  // Chuyển về JSON
  Map<String, dynamic> toJson() => {
        'dangerZoneId': dangerZoneId,
        'userId': userId,
        'tenKhuVuc': tenKhuVuc,
        'viDo': viDo,
        'kinhDo': kinhDo,
        'banKinh': banKinh,
        'moTa': moTa,
        'childrenId': childrenId,
      };
}