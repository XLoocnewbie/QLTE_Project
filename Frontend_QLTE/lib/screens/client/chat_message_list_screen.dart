import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../models/user_chat.dart';
import '../../services/chat_message_service.dart';
import 'chat_detail_screen.dart';

class ChatMessageListScreen extends StatefulWidget {
  const ChatMessageListScreen({super.key});

  @override
  State<ChatMessageListScreen> createState() => _ChatMessageListScreenState();
}

class _ChatMessageListScreenState extends State<ChatMessageListScreen> {
  late Future<List<UserChat>> _futureChats;

  @override
  void initState() {
    super.initState();
    _futureChats = _loadChats();
  }

  Future<List<UserChat>> _loadChats() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString("jwt_token") ?? "";

    if (token.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("❌ Bạn cần đăng nhập để xem tin nhắn.")),
      );
      return [];
    }

    try {
      return await ChatMessageService.getUserChats(token: token);
    } catch (e) {
      debugPrint("Lỗi tải danh sách chat: $e");
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Lỗi tải danh sách chat: $e")),
      );
      return [];
    }
  }

  Future<void> _refreshChats() async {
    setState(() {
      _futureChats = _loadChats();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("💬 Tin nhắn"),
        backgroundColor: Colors.blueAccent,
      ),
      body: RefreshIndicator(
        onRefresh: _refreshChats,
        child: FutureBuilder<List<UserChat>>(
          future: _futureChats,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            } else if (snapshot.hasError) {
              return Center(
                  child: Text("❌ Lỗi: ${snapshot.error}",
                      textAlign: TextAlign.center));
            } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
              return const Center(
                  child: Text("Không có cuộc trò chuyện nào 📭"));
            }

            final chats = snapshot.data!;
            return ListView.separated(
              padding: const EdgeInsets.all(8),
              separatorBuilder: (_, __) => const Divider(height: 1),
              itemCount: chats.length,
              itemBuilder: (context, index) {
                final chat = chats[index];
                final avatarLetter = chat.receiverEmail.isNotEmpty
                    ? chat.receiverEmail[0].toUpperCase()
                    : "?";

                return ListTile(
                  leading: CircleAvatar(
                    radius: 24,
                    backgroundColor: Colors.teal.shade300,
                    child: Text(
                      avatarLetter,
                      style: const TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  title: Text(
                    chat.receiverEmail,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  subtitle: Text(
                    chat.lastMessage.isEmpty
                        ? "(Chưa có tin nhắn)"
                        : chat.lastMessage,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  trailing: Text(
                    _formatTime(chat.lastTime),
                    style: const TextStyle(fontSize: 12, color: Colors.grey),
                  ),
                  onTap: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ChatDetailScreen(
                          adoptionRequestId: chat.adoptionRequestId,
                          receiverId: chat.receiverId,
                          receiverEmail: chat.receiverEmail,
                        ),
                      ),
                    );
                  },
                );
              },
            );
          },
        ),
      ),
    );
  }

  String _formatTime(DateTime dateTime) {
    final now = DateTime.now();
    final diff = now.difference(dateTime);

    if (diff.inDays > 0) {
      return "${dateTime.day}/${dateTime.month}";
    } else if (diff.inHours > 0) {
      return "${diff.inHours} giờ trước";
    } else if (diff.inMinutes > 0) {
      return "${diff.inMinutes} phút trước";
    } else {
      return "Vừa xong";
    }
  }
}
