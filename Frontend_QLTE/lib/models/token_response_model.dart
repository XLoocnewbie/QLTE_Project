class TokenResponse {
  final bool status;
  final String msg;
  final TokenData? data;

  TokenResponse({
    required this.status,
    required this.msg,
    this.data,
  });

  factory TokenResponse.fromJson(Map<String, dynamic> json) {
    return TokenResponse(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: json['data'] != null
          ? TokenData.fromJson(json['data'])
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'status': status,
      'msg': msg,
      'data': data?.toJson(),
    };
  }
}

class TokenData {
  final String token;
  final String? refreshToken;

  TokenData({
    required this.token,
    this.refreshToken,
  });

  factory TokenData.fromJson(Map<String, dynamic> json) {
    return TokenData(
      token: json['token'] ?? '',
      refreshToken: json['refreshToken'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'token': token,
      'refreshToken': refreshToken,
    };
  }
}
