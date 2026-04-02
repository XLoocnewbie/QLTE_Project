class UpdateDangerZoneRequest {
  final String dangerZoneId;
  final String tenKhuVuc;
  final double viDo;
  final double kinhDo;
  final double banKinh;
  final String moTa;

  UpdateDangerZoneRequest({
    required this.dangerZoneId,
    required this.tenKhuVuc,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
    required this.moTa,
  });

  Map<String, dynamic> toJson() {
    return {
      'dangerZoneId': dangerZoneId,
      'tenKhuVuc': tenKhuVuc,
      'viDo': viDo,
      'kinhDo': kinhDo,
      'banKinh': banKinh,
      'moTa': moTa,
    };
  }
}
