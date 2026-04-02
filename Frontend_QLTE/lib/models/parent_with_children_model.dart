import 'child_response_model.dart';

class ParentWithChildren {
  final String parentId;
  final String parentName;
  final String? email;
  final String? phoneNumber;
  final String? avatar; // 🧍 ảnh đại diện phụ huynh
  final List<Child> children;

  ParentWithChildren({
    required this.parentId,
    required this.parentName,
    this.email,
    this.phoneNumber,
    this.avatar,
    required this.children,
  });

  factory ParentWithChildren.fromJson(Map<String, dynamic> json) {
    // 🔹 Dữ liệu backend hiện tại trả về "avatarND"
    final avatarPath = json['avatarND']?.toString();

    return ParentWithChildren(
      parentId: (json['parentId'] ?? '').toString(),
      parentName: (json['parentName'] ?? '').toString(),
      email: json['email']?.toString(),
      phoneNumber: json['phoneNumber']?.toString(),
      avatar: (avatarPath != null && avatarPath.isNotEmpty)
          ? avatarPath
          : null,
      children: (json['children'] as List<dynamic>? ?? [])
          .map((c) => Child.fromJson(c))
          .toList(),
    );
  }
}
