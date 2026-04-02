import 'package:flutter/material.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/message/message_response_model.dart';
import 'package:frontend_qlte/screens/client/message/message_detail_screen.dart';
import 'package:frontend_qlte/services/message_service.dart';
import 'package:frontend_qlte/services/user_service.dart';
import 'package:frontend_qlte/utils/jwt_helper.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

class MessageController extends ChangeNotifier {
  List<MessageDataModel> messages = [];
  bool isLoading = false;
  String? userId;
  String? token;

  Future<void> loadMessages() async {
    isLoading = true;
    notifyListeners();

    final prefs = await SharedPreferences.getInstance();
    token = prefs.getString('jwt_token');
    if (token == null || token!.isEmpty) {
      isLoading = false;
      notifyListeners();
      return;
    }

    userId = JwtHelper.getUserId(token!);
    try {
      final response = await MessageService.getLastMessagePerUser(userId!);
      if (response.status && response.data != null) {
        messages = response.data!;
        for (var msg in messages) {
          String partnerId = msg.senderId == userId
              ? msg.receiverId
              : msg.senderId;

          final userResponse = await UserService.getUserByUserId(partnerId);
          if (userResponse != null) {
            msg.senderName = userResponse.nameND.isEmpty ? "Người dùng" : userResponse.nameND;
            msg.senderAvatar =
                (userResponse.avatarND.isNotEmpty)
                ? (userResponse.avatarND.startsWith('http')
                      ? userResponse.avatarND
                      : "${Config_URL.urlServer}${userResponse.avatarND}")
                : "";
          }
        }
      } else {
        messages = [];
      }
    } catch (e) {
      print("Lỗi loadMessages: $e");
      messages = [];
    }

    isLoading = false;
    notifyListeners();
  }
}

class MessageScreen extends StatefulWidget {
  const MessageScreen({super.key});

  @override
  State<MessageScreen> createState() => _MessageScreenState();
}

class _MessageScreenState extends State<MessageScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () =>
          Provider.of<MessageController>(context, listen: false).loadMessages(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final controller = Provider.of<MessageController>(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Tin nhắn'), centerTitle: true,automaticallyImplyLeading: false),
      body: controller.isLoading
          ? const Center(child: CircularProgressIndicator())
          : controller.messages.isEmpty
          ? const Center(child: Text('Chưa có tin nhắn nào.'))
          : ListView.builder(
              itemCount: controller.messages.length,
              padding: const EdgeInsets.all(12),
              itemBuilder: (context, index) {
                final msg = controller.messages[index];
                final timeString = DateFormat(
                  'HH:mm • dd/MM',
                ).format(msg.thoiGian);
                final userName = msg.senderName ?? "Người dùng";
                final userAvatar = msg.senderAvatar;

                return GestureDetector(
                  onTap: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (_) => ChangeNotifierProvider(
                          create: (_) => MessageDetailController(),
                          child: MessageDetailScreen(
                            currentUserId: controller.userId!,
                            otherUserId: msg.receiverId == controller.userId
                                ? msg.senderId
                                : msg.receiverId,
                            otherUserName: userName,
                            otherUserAvatar: userAvatar,
                          ),
                        ),
                      ),
                    );
                  },
                  child: Container(
                    margin: const EdgeInsets.symmetric(vertical: 8),
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.grey.withOpacity(0.1),
                          blurRadius: 4,
                          offset: const Offset(0, 3),
                        ),
                      ],
                    ),
                    child: Row(
                      children: [
                        CircleAvatar(
                          radius: 28,
                          backgroundColor: Colors.grey[300],
                          backgroundImage:
                              userAvatar != null && userAvatar.isNotEmpty
                              ? NetworkImage(userAvatar)
                              : const AssetImage(
                                      'assets/images/default_avatar.png',
                                    )
                                    as ImageProvider,
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                userName,
                                style: const TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                msg.noiDung,
                                style: const TextStyle(color: Colors.grey),
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 8),
                        Text(
                          timeString,
                          style: const TextStyle(
                            color: Colors.grey,
                            fontSize: 12,
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
    );
  }
}
