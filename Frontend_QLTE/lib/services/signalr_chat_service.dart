import 'package:signalr_netcore/signalr_client.dart';
import 'package:frontend_qlte/config/config_url.dart';
class SignalrChatService {
  
}
typedef OnReceiveMessage = void Function(
  String messageId,
  String senderId,
  String content,
);

typedef OnMessageRead = void Function(
  String messageId,
  String readerId,
);

class SignalRChatService {
  HubConnection? _connection;
  bool isConnected = false;

  OnReceiveMessage? onReceiveMessage;
  OnMessageRead? onMessageRead;

  /// Kết nối SignalR với userId
  Future<void> connect(String currentUserId) async {
    final url = "${Config_URL.urlServer}chatHub?userId=$currentUserId";
    _connection = HubConnectionBuilder()
        .withUrl(url)
        .withAutomaticReconnect()
        .build();

    _connection!.on('ReceivePrivateMessage', (args) {
      if (args != null && args.length >= 3) {
        final messageId = args[0].toString();
        final senderId = args[1] as String;
        final message = args[2] as String;
        onReceiveMessage?.call(messageId, senderId, message);
      }
    });

    _connection!.on('MessageRead', (args) {
      if (args != null && args.length >= 2) {
        final messageId = args[0].toString();
        final readerId = args[1].toString();
        onMessageRead?.call(messageId, readerId);
      }
    });

    await _connection!.start();
    isConnected = true;
    print("✅ SignalRChatService connected with $currentUserId");
  }

  /// Gửi tin nhắn
  Future<String?> sendMessage(
    String fromUserId,
    String toUserId,
    String content,
  ) async {
    if (!isConnected) return null;
    try {
      final result = await _connection!.invoke(
        'SendPrivateMessage',
        args: [fromUserId, toUserId, content],
      );
      return result?.toString();
    } catch (e) {
      print("❌ Error sending message: $e");
      return null;
    }
  }

  /// Đánh dấu tin nhắn đã đọc
  Future<void> markMessageAsRead(String messageId, String readerId) async {
    if (!isConnected) return;
    try {
      await _connection!.invoke('MarkMessageAsRead', args: [messageId, readerId]);
    } catch (e) {
      print("❌ Error marking as read: $e");
    }
  }

  /// Dừng kết nối
  Future<void> disconnect() async {
    await _connection?.stop();
    isConnected = false;
    print("🔌 SignalRChatService disconnected");
  }
}
