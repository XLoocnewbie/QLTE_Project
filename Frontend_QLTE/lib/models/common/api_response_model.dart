class ApiResponseModel {
  final bool status;
  final String msg;

  ApiResponseModel({
    required this.status,
    required this.msg,
  });

  factory ApiResponseModel.fromJson(Map<String, dynamic> json) {
    return ApiResponseModel(
      status: json['status'].toString().toLowerCase() == 'true', // xử lý "true"/"false" dạng string
      msg: json['msg'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'status': status.toString(),
      'msg': msg,
    };
  }
}
