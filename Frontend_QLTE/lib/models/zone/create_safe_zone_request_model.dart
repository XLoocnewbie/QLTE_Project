class CreateSafeZoneRequest {
  String userId;
  String childrenId;
  String tenZone;
  double viDo;
  double kinhDo;
  double banKinh;

  CreateSafeZoneRequest({
    required this.userId,
    required this.childrenId,
    required this.tenZone,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
  });

  factory CreateSafeZoneRequest.fromJson(Map<String, dynamic> json) {
    return CreateSafeZoneRequest(
      userId: json['userId'],
      childrenId: json['childrenId'],
      tenZone: json['tenZone'],
      viDo: (json['viDo'] as num).toDouble(),
      kinhDo: (json['kinhDo'] as num).toDouble(),
      banKinh: (json['banKinh'] as num).toDouble(),
    );
  }

  Map<String, dynamic> toJson() => {
        'userId': userId,
        'childrenId': childrenId,
        'tenZone': tenZone,
        'viDo': viDo,
        'kinhDo': kinhDo,
        'banKinh': banKinh,
      };
}
