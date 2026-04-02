class SOSRequest {
  final String sosId;
  final String childId;
  final DateTime thoiGian;
  final double viDo;
  final double kinhDo;
  final String trangThai;

  SOSRequest({
    required this.sosId,
    required this.childId,
    required this.thoiGian,
    required this.viDo,
    required this.kinhDo,
    required this.trangThai,
  });

  factory SOSRequest.fromJson(Map<String, dynamic> json) {
    return SOSRequest(
      sosId: json['sosId']?.toString() ?? '',
      childId: json['childId']?.toString() ?? '',
      thoiGian: DateTime.tryParse(json['thoiGian']?.toString() ?? '') ?? DateTime(2000),
      viDo: (json['viDo'] is num) ? json['viDo'].toDouble() : double.tryParse(json['viDo']?.toString() ?? '0') ?? 0,
      kinhDo: (json['kinhDo'] is num) ? json['kinhDo'].toDouble() : double.tryParse(json['kinhDo']?.toString() ?? '0') ?? 0,
      trangThai: json['trangThai']?.toString() ?? 'Đang xử lý',
    );
  }
}
