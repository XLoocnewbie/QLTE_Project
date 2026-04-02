// Model chính chứa danh sách SafeZone
class SafeZoneResponse {
  bool status;
  String msg;
  List<SafeZone> data;
  dynamic pagination; // null hoặc object, tùy API

  SafeZoneResponse({
    required this.status,
    required this.msg,
    required this.data,
    this.pagination,
  });

  factory SafeZoneResponse.fromJson(Map<String, dynamic> json) {
    var dataField = json['data'];

    List<SafeZone> parsedData = [];

    if (dataField != null) {
      if (dataField is List) {
        parsedData = List<SafeZone>.from(
          dataField.map((x) => SafeZone.fromJson(x)),
        );
      } else if (dataField is Map<String, dynamic>) {
        parsedData = [SafeZone.fromJson(dataField)];
      }
    }

    return SafeZoneResponse(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: parsedData,
      pagination: json['pagination'],
    );
  }
}

// Model cho từng SafeZone
class SafeZone {
  String safeZoneId;
  String userId;
  String tenZone;
  double viDo;
  double kinhDo;
  double banKinh;
  String childrenId;

  SafeZone({
    required this.safeZoneId,
    required this.userId,
    required this.tenZone,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
    required this.childrenId,
  });

  factory SafeZone.fromJson(Map<String, dynamic> json) {
    return SafeZone(
      safeZoneId: json['safeZoneId'],
      userId: json['userId'],
      tenZone: json['tenZone'],
      viDo: (json['viDo'] as num).toDouble(),
      kinhDo: (json['kinhDo'] as num).toDouble(),
      banKinh: (json['banKinh'] as num).toDouble(),
      childrenId: json['childrenId'],
    );
  }

  Map<String, dynamic> toJson() => {
    'safeZoneId': safeZoneId,
    'userId': userId,
    'tenZone': tenZone,
    'viDo': viDo,
    'kinhDo': kinhDo,
    'banKinh': banKinh,
    'childrenId': childrenId,
  };
}
