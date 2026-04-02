class ScheduleModel {
  final String scheduleId;
  final String childId;
  final String tenMonHoc;
  final int thu; // DayOfWeek (0=Sunday, 1=Monday,...)
  final String gioBatDau; // TimeSpan dạng "08:00:00"
  final String gioKetThuc;

  ScheduleModel({
    required this.scheduleId,
    required this.childId,
    required this.tenMonHoc,
    required this.thu,
    required this.gioBatDau,
    required this.gioKetThuc,
  });

  factory ScheduleModel.fromJson(Map<String, dynamic> json) {
    return ScheduleModel(
      scheduleId: json["scheduleId"] ?? "",
      childId: json["childId"] ?? "",
      tenMonHoc: json["tenMonHoc"] ?? "",
      thu: json["thu"] is int ? json["thu"] : int.tryParse(json["thu"].toString()) ?? 0,
      gioBatDau: json["gioBatDau"] ?? "",
      gioKetThuc: json["gioKetThuc"] ?? "",
    );
  }

  Map<String, dynamic> toJson() => {
    "scheduleId": scheduleId,
    "childId": childId,
    "tenMonHoc": tenMonHoc,
    "thu": thu,
    "gioBatDau": gioBatDau,
    "gioKetThuc": gioKetThuc,
  };
}
