import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:signalr_netcore/signalr_client.dart';
import '../../models/chat_message.dart';
import '../../services/chat_message_service.dart';

class ChatDetailScreen extends StatefulWidget {
  final int adoptionRequestId;
  final String receiverId;
  final String receiverEmail;

  const ChatDetailScreen({
    super.key,
    required this.adoptionRequestId,
    required this.receiverId,
    required this.receiverEmail,
  });

  @override
  State<ChatDetailScreen> createState() => _ChatDetailScreenState();
}

class _ChatDetailScreenState extends State<ChatDetailScreen> {
  final TextEditingController _messageController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  List<ChatMessage> _messages = [];

  String _token = "";
  String _currentUserId = "";
  HubConnection? _hubConnection;
  bool _isConnected = false;

  @override
  void initState() {
    super.initState();
    _initChat();
  }

  @override
  void dispose() {
    _hubConnection?.stop();
    _messageController.dispose();
    super.dispose();
  }

  // 🔹 Load token, tin nhắn ban đầu và kết nối SignalR
  Future<void> _initChat() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString("jwt_token") ?? "";
    _currentUserId = prefs.getString("user_id") ?? "";

    await _loadMessages();
    await _connectSignalR();
  }

  // 🔹 Kết nối tới SignalR Hub
  Future<void> _connectSignalR() async {
    final hubUrl =
        "https://firstsagecat76.conveyor.cloud/chatHub?access_token=$_token"; // ⚠️ Đổi nếu test thật: 192.168.x.x

    _hubConnection = HubConnectionBuilder()
        .withUrl(
      hubUrl,
      options: HttpConnectionOptions(
        transport: HttpTransportType.WebSockets, // ✅ viết hoa đúng enum
        skipNegotiation: true,
      ),
    )
        .withAutomaticReconnect()
        .build();

    // 🟢 Khi nhận được tin nhắn mới từ người khác
    _hubConnection!.on("ReceiveMessage", (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>;
        final msg = ChatMessage.fromJson(data);
        if (msg.adoptionRequestId == widget.adoptionRequestId) {
          setState(() => _messages.add(msg));
          _scrollToBottom();
        }
      }
    });

    // 🟢 Khi chính mình gửi tin (server phản hồi lại)
    _hubConnection!.on("MessageSent", (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>;
        final msg = ChatMessage.fromJson(data);
        if (msg.adoptionRequestId == widget.adoptionRequestId) {
          setState(() => _messages.add(msg));
          _scrollToBottom();
        }
      }
    });

    // 🟠 Log khi ngắt kết nối hoặc lỗi
    _hubConnection!.onclose(({Exception? error}) {
      debugPrint("⚠️ SignalR disconnected: ${error?.toString()}");
      setState(() => _isConnected = false);
    });

    // 🟢 Khi reconnect thành công
    _hubConnection!.onreconnected(({String? connectionId}) {
      debugPrint("🔄 SignalR reconnected: $connectionId");
      setState(() => _isConnected = true);
    });

    // 🟠 Khi đang reconnect
    _hubConnection!.onreconnecting(({Exception? error}) {
      debugPrint("⏳ Reconnecting SignalR... ${error?.toString()}");
      setState(() => _isConnected = false);
    });

    // 🧠 Bắt đầu kết nối
    await _hubConnection!.start();
    debugPrint("✅ Connected to SignalR Hub");
    setState(() => _isConnected = true);
  }

  // 🔹 Load tin nhắn ban đầu qua API
  Future<void> _loadMessages() async {
    try {
      final data = await ChatMessageService.getMessagesByAdoptionRequest(
        adoptionRequestId: widget.adoptionRequestId,
        token: _token,
      );
      setState(() => _messages = data);
      _scrollToBottom();
    } catch (e) {
      debugPrint("❌ Lỗi load tin nhắn: $e");
    }
  }

  // 🔹 Gửi tin nhắn qua SignalR (Realtime)
  // 🔹 Gửi tin nhắn qua SignalR (Realtime)
  Future<void> _sendMessage() async {
    final content = _messageController.text.trim();
    if (content.isEmpty || _hubConnection == null) return;

    final dto = {
      "adoptionRequestId": widget.adoptionRequestId,
      "noiDung": content,
    };

    try {
      // 🧩 1️⃣ Kiểm tra kết nối
      if (_hubConnection!.state != HubConnectionState.Connected) {
        debugPrint("⚠️ SignalR chưa kết nối, đang thử kết nối lại...");
        await _hubConnection!.start();
        setState(() => _isConnected = true);
      }

      // 🧩 2️⃣ Gửi tin nhắn
      await _hubConnection!.invoke("SendMessage", args: [dto]);
      _messageController.clear();
      FocusScope.of(context).unfocus();
      _scrollToBottom();
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("❌ Lỗi gửi tin nhắn: $e")),
      );
    }
  }

  // 🔹 Tự động scroll xuống cuối khi có tin mới
  void _scrollToBottom() {
    Future.delayed(const Duration(milliseconds: 200), () {
      if (_scrollController.hasClients) {
        _scrollController.jumpTo(_scrollController.position.maxScrollExtent);
      }
    });
  }

  // 🔹 UI bong bóng chat
  Widget _buildMessageBubble(ChatMessage message) {
    final isMine = message.senderId == _currentUserId;

    return Align(
      alignment: isMine ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4, horizontal: 10),
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: isMine ? Colors.teal.shade300 : Colors.grey.shade300,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(12),
            topRight: const Radius.circular(12),
            bottomLeft: Radius.circular(isMine ? 12 : 0),
            bottomRight: Radius.circular(isMine ? 0 : 12),
          ),
        ),
        child: Text(
          message.noiDung,
          style: TextStyle(
            color: isMine ? Colors.white : Colors.black87,
          ),
        ),
      ),
    );
  }

  // 🔹 Danh sách tin nhắn
  Widget _buildChatList() {
    if (_messages.isEmpty) {
      return const Center(child: Text("Chưa có tin nhắn nào 📨"));
    }

    return ListView.builder(
      controller: _scrollController,
      padding: const EdgeInsets.symmetric(vertical: 10),
      itemCount: _messages.length,
      itemBuilder: (context, index) {
        return _buildMessageBubble(_messages[index]);
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Row(
          children: [
            CircleAvatar(
              radius: 18,
              backgroundColor: Colors.teal.shade400,
              child: Text(
                widget.receiverEmail.isNotEmpty
                    ? widget.receiverEmail[0].toUpperCase()
                    : "?",
                style: const TextStyle(color: Colors.white),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Text(
                widget.receiverEmail,
                style:
                const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                overflow: TextOverflow.ellipsis,
              ),
            ),
            const SizedBox(width: 8),
            Icon(
              _isConnected ? Icons.circle : Icons.circle_outlined,
              size: 12,
              color: _isConnected ? Colors.green : Colors.red,
            ),
          ],
        ),
        backgroundColor: Colors.blueAccent,
      ),
      body: Column(
        children: [
          Expanded(child: _buildChatList()),
          const Divider(height: 1),
          SafeArea(
            child: Container(
              padding:
              const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
              color: Colors.white,
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _messageController,
                      decoration: const InputDecoration(
                        hintText: "Nhập tin nhắn...",
                        border: InputBorder.none,
                      ),
                      textInputAction: TextInputAction.send,
                      onSubmitted: (_) => _sendMessage(),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.send, color: Colors.teal),
                    onPressed: _sendMessage,
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