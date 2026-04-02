class RefreshTokenResponse {
  final bool status;
  final String msg;
  final List<RefreshTokenInfo> data;
  final Pagination? pagination;

  RefreshTokenResponse({
    required this.status,
    required this.msg,
    required this.data,
    this.pagination,
  });

  factory RefreshTokenResponse.fromJson(Map<String, dynamic> json) {
    return RefreshTokenResponse(
      status: json['status'] ?? false,
      msg: json['msg'] ?? '',
      data: (json['data'] as List<dynamic>? ?? [])
          .map((item) => RefreshTokenInfo.fromJson(item))
          .toList(),
      pagination: json['pagination'] != null
          ? Pagination.fromJson(json['pagination'])
          : null,
    );
  }
}

class RefreshTokenInfo {
  final String userId;
  final String? email;
  final String token;
  final String? revokedAt;

  RefreshTokenInfo({
    required this.userId,
    this.email,
    required this.token,
    this.revokedAt,
  });

  factory RefreshTokenInfo.fromJson(Map<String, dynamic> json) {
    return RefreshTokenInfo(
      userId: json['userId'] ?? '',
      email: json['email'],
      token: json['token'] ?? '',
      revokedAt: json['revokedAt']?.toString(),
    );
  }
}

class Pagination {
  final int page;
  final int last;
  final int limit;
  final int total;

  Pagination({
    required this.page,
    required this.last,
    required this.limit,
    required this.total,
  });

  factory Pagination.fromJson(Map<String, dynamic> json) {
    return Pagination(
      page: json['page'] ?? 1,
      last: json['last'] ?? 1,
      limit: json['limit'] ?? 10,
      total: json['total'] ?? 0,
    );
  }
}
