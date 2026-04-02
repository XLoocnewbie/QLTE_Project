import 'package:flutter/material.dart';
import 'package:flutter/scheduler.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:frontend_qlte/models/message/message_model.dart';
import 'package:frontend_qlte/models/message/user_model.dart';
import 'package:frontend_qlte/screens/client/message/call_screen.dart';
import 'package:frontend_qlte/services/message_service.dart';
import 'package:frontend_qlte/services/signalr_chat_service.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:provider/provider.dart';
import 'package:speech_to_text/speech_to_text.dart';

/// -------------------- CONTROLLER --------------------

class MessageDetailController extends ChangeNotifier {
  final SignalRChatService _chatService = SignalRChatService();
  List<MessageModel> messages = [];
  final Set<String> _markedMessages = {};

  String? currentUserId;
  String? otherUserId;
  String? currentUserName;
  String? currentUserAvatar;
  String? otherUserName;
  String? otherUserAvatar;

  bool isLoading = false;
  bool isLoadingMore = false;
  int currentPage = 1;
  final int pageSize = 50;

  ScrollController scrollController = ScrollController();

  MessageDetailController() {
    _chatService.onReceiveMessage = _handleReceiveMessage;
    _chatService.onMessageRead = _handleMessageRead;
  }

  void scrollToBottom() {
    SchedulerBinding.instance.addPostFrameCallback((_) {
      if (scrollController.hasClients) {
        scrollController.animateTo(
          scrollController.position.maxScrollExtent,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  Future<void> initConnection(
    String currentId,
    String otherId, {
    String? currentName,
    String? currentAvatar,
    String? otherName,
    String? otherAvatar,
  }) async {
    currentUserId = currentId;
    otherUserId = otherId;
    currentUserName = currentName ?? "Bạn";
    currentUserAvatar = currentAvatar;
    otherUserName = otherName ?? "Người dùng";
    otherUserAvatar = otherAvatar;

    await _chatService.connect(currentId);
    await loadMessages();
  }

  void _handleReceiveMessage(
    String messageId,
    String senderId,
    String content,
  ) {
    final message = MessageModel(
      messageID: messageId,
      content: content,
      seen: false,
      type: "Text",
      timestamp: DateTime.now(),
      fromUser: UserModel(
        id: senderId,
        fullName: otherUserName ?? '',
        avatar: otherUserAvatar ?? '',
        userName: '',
      ),
      toUser: UserModel(
        id: currentUserId ?? '',
        fullName: currentUserName ?? '',
        avatar: currentUserAvatar ?? '',
        userName: '',
      ),
    );
    messages.add(message);
    notifyListeners();
    scrollToBottom();
  }

  void _handleMessageRead(String messageId, String readerId) {
    final index = messages.indexWhere((m) => m.messageID == messageId);
    if (index != -1) {
      messages[index] = messages[index].copyWith(seen: true);
      notifyListeners();
    }
  }

  Future<void> loadMessages({bool loadMore = false}) async {
    if (currentUserId == null || otherUserId == null) return;

    if (loadMore) {
      isLoadingMore = true;
    } else {
      isLoading = true;
      currentPage = 1;
      messages.clear();
    }
    notifyListeners();

    final response = await MessageService.getHistory(
      currentUserId!,
      otherUserId!,
      currentPage,
      pageSize,
    );

    final messagesModel = response.messages;
    if (messagesModel.status && messagesModel.data.isNotEmpty) {
      final List<MessageModel> newMessages = messagesModel.data;
      if (loadMore) {
        messages.insertAll(0, newMessages);
      } else {
        messages.addAll(newMessages);
        scrollToBottom();
      }
      currentPage++;
    }

    isLoading = false;
    isLoadingMore = false;
    notifyListeners();
  }

  Future<void> sendMessage(String content) async {
    if (content.isEmpty || !_chatService.isConnected) return;
    final realMessageId = await _chatService.sendMessage(
      currentUserId!,
      otherUserId!,
      content,
    );

    if (realMessageId == null) return;

    messages.add(
      MessageModel(
        messageID: realMessageId,
        content: content,
        seen: false,
        type: "Text",
        timestamp: DateTime.now(),
        fromUser: UserModel(
          id: currentUserId!,
          fullName: currentUserName!,
          avatar: currentUserAvatar ?? '',
          userName: '',
        ),
        toUser: UserModel(
          id: otherUserId!,
          fullName: otherUserName!,
          avatar: otherUserAvatar ?? '',
          userName: '',
        ),
      ),
    );

    notifyListeners();
    scrollToBottom();
  }

  Future<void> markMessageAsRead(String messageId) async {
    if (_markedMessages.contains(messageId)) return; // tránh gửi trùng
    _markedMessages.add(messageId);
    await _chatService.markMessageAsRead(messageId, currentUserId!);
  }

  Future<void> disposeConnection() async {
    await _chatService.disconnect();
  }
}

/// -------------------- SCREEN --------------------
class MessageDetailScreen extends StatefulWidget {
  final String currentUserId;
  final String otherUserId;
  final String otherUserName;
  final String? currentUserAvatar;
  final String? otherUserAvatar;

  const MessageDetailScreen({
    super.key,
    required this.currentUserId,
    required this.otherUserId,
    required this.otherUserName,
    this.currentUserAvatar,
    this.otherUserAvatar,
  });

  @override
  State<MessageDetailScreen> createState() => _MessageDetailScreenState();
}

class _MessageDetailScreenState extends State<MessageDetailScreen> {
  final TextEditingController _textController = TextEditingController();
  late SpeechToText _speech;
  bool _isListening = false;
  bool _speechAvailable = false;

  String getAvatarUrl(String? avatar) {
    if (avatar == null || avatar.isEmpty || avatar == "null") {
      return "";
    }
    if (avatar.startsWith('http')) return avatar;
    return '${Config_URL.urlServer}$avatar';
  }

  Future<void> _initSpeech() async {
    // 1️⃣ Xin quyền microphone
    PermissionStatus status = await Permission.microphone.request();
    if (!status.isGranted) {
      print("❌ Quyền microphone bị từ chối");
      return;
    }

    // 2️⃣ Khởi tạo SpeechToText
    _speech = SpeechToText();

    _speechAvailable = await _speech.initialize(
      onStatus: (val) {
        print('Speech status: $val');
        // Tự động cập nhật icon micro
        setState(() => _isListening = val == 'listening');
      },
      onError: (val) => print('Speech error: $val'),
    );

    if (!_speechAvailable) {
      print("❌ Microphone không khả dụng");
    } else {
      print("✅ Microphone sẵn sàng");
    }
  }

  void _startListening() {
    if (!_speech.isAvailable) return;
    setState(() => _isListening = true);

    _speech.listen(
      onResult: (val) {
        setState(() {
          _textController.text = val.recognizedWords;
        });
      },
      localeId: 'vi_VN', // nhận tiếng Việt
      listenFor: const Duration(minutes: 1),
      listenOptions: SpeechListenOptions(
        partialResults: true, // nhận kết quả từng phần
        cancelOnError: true, // thay cho cancelOnError deprecated
      ),
    );
  }

  void _stopListening() {
    _speech.stop();
    setState(() => _isListening = false); // nút micro trở về màu bình thường
  }

  @override
  void initState() {
    super.initState();
    _speech = SpeechToText();
    _initSpeech();

    Future.microtask(
      () => Provider.of<MessageDetailController>(context, listen: false)
          .initConnection(
            widget.currentUserId,
            widget.otherUserId,
            currentName: "Bạn",
            currentAvatar: widget.currentUserAvatar,
            otherName: widget.otherUserName,
            otherAvatar: widget.otherUserAvatar,
          ),
    );
  }

  @override
  void dispose() {
    Provider.of<MessageDetailController>(
      context,
      listen: false,
    ).disposeConnection();
    _textController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final controller = Provider.of<MessageDetailController>(context);

    return Scaffold(
      appBar: AppBar(
        title: Row(
          children: [
            CircleAvatar(
              backgroundImage:
                  widget.otherUserAvatar != null &&
                      widget.otherUserAvatar!.isNotEmpty
                  ? NetworkImage(getAvatarUrl(widget.otherUserAvatar))
                  : const AssetImage('assets/images/default_avatar.png')
                        as ImageProvider,
              radius: 18,
            ),
            const SizedBox(width: 10),
            Text(widget.otherUserName),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.videocam, color: Colors.white),
            onPressed: () {
              // Chuyển sang màn hình Video Call
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => CallScreen(
                    currentUserId: widget.currentUserId,
                    otherUserId: widget.otherUserId,
                    type: 'video', // video call
                  ),
                ),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.call, color: Colors.white),
            onPressed: () {
              // Chuyển sang màn hình Voice Call
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => CallScreen(
                    currentUserId: widget.currentUserId,
                    otherUserId: widget.otherUserId,
                    type: 'audio', // voice call
                  ),
                ),
              );
            },
          ),
        ],
      ),
      body: Column(
        children: [
          Expanded(
            child: controller.isLoading
                ? const Center(child: CircularProgressIndicator())
                : ListView.builder(
                    reverse: false,
                    controller: controller.scrollController,
                    padding: const EdgeInsets.all(12),
                    itemCount: controller.messages.length,
                    itemBuilder: (context, index) {
                      final msg = controller.messages[index];
                      final isMe = msg.fromUser.id == widget.currentUserId;
                      // Tìm tin nhắn cuối cùng mà user hiện tại gửi
                      final lastMyMessageIndex = controller.messages
                          .lastIndexWhere(
                            (m) => m.fromUser.id == widget.currentUserId,
                          );
                      // Nếu user hiện tại là người nhận và tin nhắn chưa đọc → đánh dấu đã đọc
                      if (!isMe && !msg.seen) {
                        // Dùng postFrameCallback để gọi sau khi UI build xong
                        SchedulerBinding.instance.addPostFrameCallback((_) {
                          controller.markMessageAsRead(msg.messageID);
                        });
                      }
                      bool showAvatarAndName = true;
                      if (index > 0) {
                        final prevMsg = controller.messages[index - 1];
                        if (prevMsg.fromUser.id == msg.fromUser.id) {
                          showAvatarAndName = false;
                        }
                      }

                      return Column(
                        crossAxisAlignment: isMe
                            ? CrossAxisAlignment.end
                            : CrossAxisAlignment.start,
                        children: [
                          if (showAvatarAndName)
                            Row(
                              mainAxisAlignment: isMe
                                  ? MainAxisAlignment.end
                                  : MainAxisAlignment.start,
                              children: [
                                if (!isMe)
                                  CircleAvatar(
                                    backgroundImage:
                                        (msg.fromUser.avatar != null &&
                                            msg.fromUser.avatar!.isNotEmpty &&
                                            msg.fromUser.avatar != "null")
                                        ? NetworkImage(
                                            getAvatarUrl(msg.fromUser.avatar),
                                          )
                                        : const AssetImage(
                                                'assets/images/default_avatar.png',
                                              )
                                              as ImageProvider,
                                    radius: 16,
                                  ),
                                const SizedBox(width: 8),
                                Text(
                                  msg.fromUser.fullName.isNotEmpty
                                      ? msg.fromUser.fullName
                                      : "Người Dùng",
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 13,
                                  ),
                                ),
                              ],
                            ),
                          Container(
                            margin: const EdgeInsets.symmetric(vertical: 3),
                            padding: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              color: isMe
                                  ? Colors.blueAccent.withOpacity(0.8)
                                  : Colors.grey.shade300,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Text(
                              msg.content,
                              style: TextStyle(
                                color: isMe ? Colors.white : Colors.black87,
                              ),
                            ),
                          ),
                          // 👇 Thêm phần này ngay sau Container:
                          if (isMe && index == lastMyMessageIndex)
                            Padding(
                              padding: const EdgeInsets.only(top: 2, right: 12),
                              child: Text(
                                msg.seen ? "Đã xem" : "Đã gửi",
                                style: TextStyle(
                                  color: msg.seen ? Colors.blue : Colors.grey,
                                  fontSize: 11,
                                ),
                              ),
                            ),
                        ],
                      );
                    },
                  ),
          ),
          if (controller.isLoadingMore)
            const Padding(
              padding: EdgeInsets.all(8),
              child: CircularProgressIndicator(strokeWidth: 2),
            ),
          SafeArea(
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
              color: Colors.grey.shade100,
              child: Row(
                children: [
                  IconButton(
                    icon: Icon(
                      _isListening ? Icons.mic : Icons.mic_none,
                      color: _isListening
                          ? Colors.red
                          : Colors.blue, // đỏ khi đang ghi âm
                    ),
                    onPressed: () {
                      if (_isListening) {
                        _stopListening(); // dừng -> màu nút xanh
                      } else {
                        _startListening(); // bắt đầu -> màu đỏ
                      }
                    },
                  ),
                  Expanded(
                    child: TextField(
                      controller: _textController,
                      decoration: const InputDecoration(
                        hintText: 'Nhập tin nhắn...',
                        border: OutlineInputBorder(),
                        isDense: true,
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  IconButton(
                    icon: const Icon(Icons.send, color: Colors.blue),
                    onPressed: () async {
                      if (_textController.text.trim().isEmpty) return;
                      await controller.sendMessage(_textController.text);
                      _textController.clear();
                    },
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
