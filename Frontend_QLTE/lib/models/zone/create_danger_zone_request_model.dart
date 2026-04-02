class CreateDangerZoneRequest {
  String userId;
  String childrenId;
  String tenKhuVuc;
  String mota;
  double viDo;
  double kinhDo;
  double banKinh;

  CreateDangerZoneRequest({
    required this.userId,
    required this.childrenId,
    required this.tenKhuVuc,
    required this.mota,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
  });

  factory CreateDangerZoneRequest.fromJson(Map<String, dynamic> json) {
    return CreateDangerZoneRequest(
      userId: json['userId'],
      childrenId: json['childrenId'],
      tenKhuVuc: json['tenKhuVuc'],
      mota: json['mota'],
      viDo: (json['viDo'] as num).toDouble(),
      kinhDo: (json['kinhDo'] as num).toDouble(),
      banKinh: (json['banKinh'] as num).toDouble(),
    );
  }

  Map<String, dynamic> toJson() => {
        'userId': userId,
        'childrenId': childrenId,
        'tenKhuVuc': tenKhuVuc,
        'mota': mota,
        'viDo': viDo,
        'kinhDo': kinhDo,
        'banKinh': banKinh,
      };
}
