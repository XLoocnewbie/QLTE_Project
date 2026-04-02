class StudyPeriod {
  final String studyPeriodId;
  final String childId;
  final String? moTa;
  final bool isActive;
  final String repeatPattern;
  final String startTime;
  final String endTime;
  final String ngayTao;

  StudyPeriod({
    required this.studyPeriodId,
    required this.childId,
    this.moTa,
    required this.isActive,
    required this.repeatPattern,
    required this.startTime,
    required this.endTime,
    required this.ngayTao,
  });

  factory StudyPeriod.fromJson(Map<String, dynamic> json) {
    return StudyPeriod(
      studyPeriodId: json['studyPeriodId'] ?? '',
      childId: json['childId'] ?? '',
      moTa: json['moTa'],
      isActive: json['isActive'] ?? false,
      repeatPattern: json['repeatPattern'] ?? 'Daily',
      startTime: json['startTime'] ?? '',
      endTime: json['endTime'] ?? '',
      ngayTao: json['ngayTao'] ?? '',
    );
  }

  Map<String, dynamic> toJson() => {
    'studyPeriodId': studyPeriodId,
    'childId': childId,
    'moTa': moTa,
    'isActive': isActive,
    'repeatPattern': repeatPattern,
    'startTime': startTime,
    'endTime': endTime,
    'ngayTao': ngayTao,
  };

  StudyPeriod copyWith({
    String? studyPeriodId,
    String? childId,
    String? moTa,
    bool? isActive,
    String? repeatPattern,
    String? startTime,
    String? endTime,
    String? ngayTao,
  }) {
    return StudyPeriod(
      studyPeriodId: studyPeriodId ?? this.studyPeriodId,
      childId: childId ?? this.childId,
      moTa: moTa ?? this.moTa,
      isActive: isActive ?? this.isActive,
      repeatPattern: repeatPattern ?? this.repeatPattern,
      startTime: startTime ?? this.startTime,
      endTime: endTime ?? this.endTime,
      ngayTao: ngayTao ?? this.ngayTao,
    );
  }
}
