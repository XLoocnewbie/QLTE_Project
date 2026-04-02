class LocationHistoryResponseModel {
  final bool status;
  final String msg;
  final List<LocationHistoryData> data;

  LocationHistoryResponseModel({
    required this.status,
    required this.msg,
    required this.data,
  });

  factory LocationHistoryResponseModel.fromJson(Map<String, dynamic> json) {
    List<LocationHistoryData> parsedData = [];

    if (json['data'] != null) {
      if (json['data'] is List) {
        // Trường hợp trả về list
        parsedData = List<LocationHistoryData>.from(
            (json['data'] as List).map((x) => LocationHistoryData.fromJson(x)));
      } else if (json['data'] is Map) {
        // Trường hợp trả về object đơn
        parsedData = [LocationHistoryData.fromJson(json['data'])];
      }
    }

    return LocationHistoryResponseModel(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: parsedData,
    );
  }

  Map<String, dynamic> toJson() => {
        'status': status,
        'msg': msg,
        'data': data.map((x) => x.toJson()).toList(),
      };
}

class LocationHistoryData {
  final String locationId;
  final String childId;
  final double viDo;
  final double kinhDo;
  final DateTime thoiGian;
  final int doChinhXac;

  LocationHistoryData({
    required this.locationId,
    required this.childId,
    required this.viDo,
    required this.kinhDo,
    required this.thoiGian,
    required this.doChinhXac,
  });

  factory LocationHistoryData.fromJson(Map<String, dynamic> json) {
    return LocationHistoryData(
      locationId: json['locationId'] ?? '',
      childId: json['childId'] ?? '',
      viDo: (json['viDo'] ?? 0).toDouble(),
      kinhDo: (json['kinhDo'] ?? 0).toDouble(),
      thoiGian: DateTime.parse(json['thoiGian']),
      doChinhXac: json['doChinhXac'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() => {
        'locationId': locationId,
        'childId': childId,
        'viDo': viDo,
        'kinhDo': kinhDo,
        'thoiGian': thoiGian.toIso8601String(),
        'doChinhXac': doChinhXac,
      };
}
