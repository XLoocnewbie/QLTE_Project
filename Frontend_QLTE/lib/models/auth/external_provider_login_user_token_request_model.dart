class ExternalProviderLoginUserRequestTokenModel {
  final String provider;
  final String idToken;

  ExternalProviderLoginUserRequestTokenModel({
    required this.provider,
    required this.idToken,
  });

  // Tạo từ JSON
  factory ExternalProviderLoginUserRequestTokenModel.fromJson(Map<String, dynamic> json) {
    return ExternalProviderLoginUserRequestTokenModel(
      provider: json['provider'] as String? ?? '',
      idToken: json['idToken'] as String? ?? '',
    );
  }

  // Chuyển về JSON
  Map<String, dynamic> toJson() {
    return {
      'provider': provider,
      'idToken': idToken,
    };
  }

  // copyWith tiện lợi (không bắt buộc)
  ExternalProviderLoginUserRequestTokenModel copyWith({String? provider, String? idToken}) {
    return ExternalProviderLoginUserRequestTokenModel(
      provider: provider ?? this.provider,
      idToken: idToken ?? this.idToken,
    );
  }

  @override
  String toString() => 'AuthToken(provider: $provider, idToken: $idToken)';
}