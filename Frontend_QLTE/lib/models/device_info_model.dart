class DeviceInfoModel {
  final String deviceId;
  final String childId;
  final String? tenThietBi;
  final String? imei;
  final int? pin;
  final bool trangThaiOnline;
  final DateTime lanCapNhatCuoi;
  final bool isTracking;
  final bool isLocked;

  DeviceInfoModel({
    required this.deviceId,
    required this.childId,
    this.tenThietBi,
    this.imei,
    this.pin,
    required this.trangThaiOnline,
    required this.lanCapNhatCuoi,
    this.isTracking = false,
    this.isLocked = false,
  });

  factory DeviceInfoModel.fromJson(Map<String, dynamic> json) {
    return DeviceInfoModel(
      deviceId: json['deviceId']?.toString() ?? '',
      childId: json['childId']?.toString() ?? '',
      tenThietBi: json['tenThietBi']?.toString(),
      imei: json['imei']?.toString(),
      pin: json['pin'] is int
          ? json['pin']
          : int.tryParse(json['pin']?.toString() ?? ''),
      trangThaiOnline: json['trangThaiOnline'] ?? false,
      lanCapNhatCuoi: DateTime.tryParse(json['lanCapNhatCuoi']?.toString() ?? '') ??
          DateTime(2000),
      isTracking: json['isTracking'] ?? false,
      isLocked: json['isLocked'] ?? false,
    );
  }

  Map<String, dynamic> toJson() => {
    "deviceId": deviceId,
    "childId": childId,
    "tenThietBi": tenThietBi,
    "imei": imei,
    "pin": pin,
    "trangThaiOnline": trangThaiOnline,
    "lanCapNhatCuoi": lanCapNhatCuoi.toIso8601String(),
    "isTracking": isTracking,
    "isLocked": isLocked,
  };
}
