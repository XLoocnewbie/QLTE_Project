class DeviceRestrictionModel {
  final String? restrictionId;
  final String deviceId;
  final String? blockedApps;
  final String? blockedDomains;
  final String? allowedDomains;
  final bool isFirewallEnabled;
  final String mode;
  final DateTime? updatedAt; // ✅ Cho phép null

  DeviceRestrictionModel({
    this.restrictionId,
    required this.deviceId,
    this.blockedApps,
    this.blockedDomains,
    this.allowedDomains,
    required this.isFirewallEnabled,
    required this.mode,
    this.updatedAt, // ✅ Không bắt buộc
  });

  factory DeviceRestrictionModel.fromJson(Map<String, dynamic> json) {
    return DeviceRestrictionModel(
      restrictionId: json['restrictionId'] as String?,
      deviceId: json['deviceId'] ?? '',
      blockedApps: json['blockedApps'] as String?,
      blockedDomains: json['blockedDomains'] as String?,
      allowedDomains: json['allowedDomains'] as String?,
      isFirewallEnabled: json['isFirewallEnabled'] ?? false,
      mode: json['mode'] ?? 'Custom',
      updatedAt: json['updatedAt'] != null
          ? DateTime.tryParse(json['updatedAt'])
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'restrictionId': restrictionId,
      'deviceId': deviceId,
      'blockedApps': blockedApps,
      'blockedDomains': blockedDomains,
      'allowedDomains': allowedDomains,
      'isFirewallEnabled': isFirewallEnabled,
      'mode': mode,
      'updatedAt': updatedAt?.toIso8601String(),
    };
  }
}
