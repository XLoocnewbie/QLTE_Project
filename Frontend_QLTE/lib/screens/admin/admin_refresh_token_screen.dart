import 'package:flutter/material.dart';
import 'package:frontend_qlte/models/refresh_token_info_model.dart';
import 'package:frontend_qlte/services/auth_service.dart';
import 'package:frontend_qlte/models/common/api_response_model.dart';

class AdminRefreshTokenScreen extends StatefulWidget {
  const AdminRefreshTokenScreen({super.key});

  @override
  State<AdminRefreshTokenScreen> createState() => _AdminRefreshTokenScreenState();
}

class _AdminRefreshTokenScreenState extends State<AdminRefreshTokenScreen> {
  bool _isLoading = true;
  List<RefreshTokenInfo> _tokens = [];
  int _page = 1;
  int _limit = 10;
  Pagination? _pagination;

  @override
  void initState() {
    super.initState();
    _loadTokens();
  }

  Future<void> _loadTokens() async {
    setState(() => _isLoading = true);
    final response = await AuthService.getAllRefreshTokens(
      page: _page,
      limit: _limit,
    );

    if (response.status) {
      setState(() {
        _tokens = response.data;
        _pagination = response.pagination;
      });
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(response.msg)),
      );
    }
    setState(() => _isLoading = false);
  }

  Future<void> _revokeToken(String refreshToken, String email) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Xác nhận khoá token"),
        content: Text("Bạn có chắc muốn khoá token của $email không?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text("Huỷ"),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text("Đồng ý"),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    // Gọi API revoke
    final ApiResponseModel result = await AuthService.RevokeToken(refreshToken);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(result.msg)),
    );

    // Sau khi revoke thành công => refresh lại danh sách
    if (result.status) {
      _loadTokens();
    }
  }

  Widget _buildLockIcon(RefreshTokenInfo token) {
    final bool isRevoked = token.revokedAt != null && token.revokedAt!.isNotEmpty;

    if (!isRevoked) {
      return IconButton(
        icon: const Icon(Icons.lock_open, color: Colors.orange),
        tooltip: "Khoá token lại",
        onPressed: () => _revokeToken(token.token, token.email ?? token.userId),
      );
    } else {
      return const Icon(Icons.lock, color: Colors.grey);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("🔐 Quản lý Refresh Token"),
        backgroundColor: Colors.indigo,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: "Tải lại",
            onPressed: _loadTokens,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _tokens.isEmpty
          ? const Center(child: Text("Không có Refresh Token nào"))
          : RefreshIndicator(
        onRefresh: _loadTokens,
        child: ListView.builder(
          itemCount: _tokens.length,
          itemBuilder: (context, index) {
            final token = _tokens[index];
            final bool isRevoked =
                token.revokedAt != null && token.revokedAt!.isNotEmpty;

            return Card(
              margin:
              const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              elevation: 3,
              child: ListTile(
                leading: Icon(
                  Icons.email,
                  color: isRevoked ? Colors.grey : Colors.blueAccent,
                ),
                title: Text(
                  token.email ?? token.userId,
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
                subtitle: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "Token: ${token.token.substring(0, 25)}...",
                      style: const TextStyle(fontSize: 13),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      "Trạng thái: ${isRevoked ? token.revokedAt : "Chưa revoke"}",
                      style: TextStyle(
                        color: isRevoked ? Colors.red : Colors.green,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
                trailing: _buildLockIcon(token),
              ),
            );
          },
        ),
      ),
      bottomNavigationBar: _pagination == null
          ? null
          : Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text("Trang ${_pagination!.page}/${_pagination!.last}"),
            Row(
              children: [
                IconButton(
                  icon: const Icon(Icons.arrow_back),
                  onPressed: _page > 1
                      ? () {
                    setState(() => _page--);
                    _loadTokens();
                  }
                      : null,
                ),
                IconButton(
                  icon: const Icon(Icons.arrow_forward),
                  onPressed: _page < _pagination!.last
                      ? () {
                    setState(() => _page++);
                    _loadTokens();
                  }
                      : null,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
