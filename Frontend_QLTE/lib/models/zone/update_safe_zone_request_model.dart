class UpdateSafeZoneRequest {
  final String safeZoneId;
  final String tenZone;
  final double viDo;
  final double kinhDo;
  final double banKinh;

  UpdateSafeZoneRequest({
    required this.safeZoneId,
    required this.tenZone,
    required this.viDo,
    required this.kinhDo,
    required this.banKinh,
  });

  Map<String, dynamic> toJson() {
    return {
      'safeZoneId': safeZoneId,
      'tenZone': tenZone,
      'viDo': viDo,
      'kinhDo': kinhDo,
      'banKinh': banKinh,
    };
  }
}
