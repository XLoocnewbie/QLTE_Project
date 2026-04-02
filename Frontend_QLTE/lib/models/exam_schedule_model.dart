class ExamSchedule {
  final String examId;
  final String childId;
  final String monThi;
  final DateTime thoiGianThi;
  final String? ghiChu;
  final bool daThiXong;
  final DateTime ngayTao;

  ExamSchedule({
    required this.examId,
    required this.childId,
    required this.monThi,
    required this.thoiGianThi,
    this.ghiChu,
    required this.daThiXong,
    required this.ngayTao,
  });

  factory ExamSchedule.fromJson(Map<String, dynamic> json) {
    return ExamSchedule(
      examId: json['examId']?.toString() ?? '',
      childId: json['childId']?.toString() ?? '',
      monThi: json['monThi']?.toString() ?? '',
      thoiGianThi: DateTime.tryParse(json['thoiGianThi']?.toString() ?? '') ?? DateTime(2000),
      ghiChu: json['ghiChu']?.toString(),
      daThiXong: json['daThiXong'] ?? false,
      ngayTao: DateTime.tryParse(json['ngayTao']?.toString() ?? '') ?? DateTime.now(),
    );
  }
}
