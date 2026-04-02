// call_screen.dart
import 'package:flutter/material.dart';
import 'package:flutter_webrtc/flutter_webrtc.dart';
import 'package:frontend_qlte/config/config_url.dart';
import 'package:signalr_netcore/signalr_client.dart';

class CallScreen extends StatefulWidget {
  final String currentUserId;
  final String otherUserId;
  final String type; // 'video' hoặc 'audio'

  const CallScreen({
    super.key,
    required this.currentUserId,
    required this.otherUserId,
    required this.type,
  });

  @override
  State<CallScreen> createState() => _CallScreenState();
}

class _CallScreenState extends State<CallScreen> {
  final RTCVideoRenderer _localRenderer = RTCVideoRenderer();
  final RTCVideoRenderer _remoteRenderer = RTCVideoRenderer();
  MediaStream? _localStream;
  RTCPeerConnection? _peerConnection;

  late HubConnection _hubConnection;
  bool inCall = false;

  @override
  void initState() {
    super.initState();
    _initRenderers();
    _initSignalR();
  }

  Future<void> _initRenderers() async {
    await _localRenderer.initialize();
    await _remoteRenderer.initialize();
  }

  Future<void> _initSignalR() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(
          '${Config_URL.urlServer}chathub?userId=${widget.currentUserId}',
        )
        .build();

    // Nhận offer từ người khác
    _hubConnection.on("ReceiveOffer", (args) async {
      final fromUserId = args?[0] as String;
      final sdp = args?[1] as String;
      final type = args?[2] as String;
      await _handleOffer(fromUserId, sdp, type);
    });

    // Nhận answer
    _hubConnection.on("ReceiveAnswer", (args) async {
      final sdp = args?[1] as String?;
      if (_peerConnection != null) {
        await _peerConnection!.setRemoteDescription(
          RTCSessionDescription(sdp, 'answer'),
        );
      }
    });

    // Nhận ICE candidate
    _hubConnection.on("ReceiveIceCandidate", (args) async {
      if (args == null || args.length < 2) return;

      final candidateMap =
          args[1] as Map<String, dynamic>?; // cast từ Object sang Map
      if (candidateMap != null && _peerConnection != null) {
        await _peerConnection!.addCandidate(
          RTCIceCandidate(
            candidateMap['candidate'] as String?,
            candidateMap['sdpMid'] as String?,
            candidateMap['sdpMLineIndex'] as int?,
          ),
        );
      }
    });

    await _hubConnection.start();
    debugPrint("✅ SignalR connected");
  }

  Future<void> _startCall() async {
    await _createLocalStream();

    _peerConnection = await _createPeerConnection();

    // Tạo offer
    final offer = await _peerConnection!.createOffer();
    await _peerConnection!.setLocalDescription(offer);

    // Gửi offer qua SignalR
    await _hubConnection.invoke(
      "SendOffer",
      args: <Object>[
        widget.currentUserId ,
        widget.otherUserId ,
        offer.sdp ?? "",
        widget.type,
      ],
    );

    setState(() => inCall = true);
  }

  Future<void> _createLocalStream() async {
    final mediaConstraints = widget.type == 'video'
        ? {'audio': true, 'video': true}
        : {'audio': true, 'video': false};

    _localStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
    _localRenderer.srcObject = _localStream;
  }

  Future<RTCPeerConnection> _createPeerConnection() async {
    final configuration = {
      'iceServers': [
        {'urls': 'stun:stun.l.google.com:19302'},
      ],
    };

    final pc = await createPeerConnection(configuration);

    _localStream?.getTracks().forEach((track) {
      pc.addTrack(track, _localStream!);
    });

    pc.onTrack = (event) {
      if (event.streams.isNotEmpty) {
        setState(() {
          _remoteRenderer.srcObject = event.streams[0];
        });
      }
    };

    pc.onIceCandidate = (candidate) {
      if (candidate != null) {
        _hubConnection.invoke(
          "SendIce",
          args: [widget.currentUserId, widget.otherUserId, candidate.toMap()],
        );
      }
    };

    return pc;
  }

  Future<void> _handleOffer(String fromUserId, String sdp, String type) async {
    await _createLocalStream();

    _peerConnection ??= await _createPeerConnection();

    await _peerConnection!.setRemoteDescription(
      RTCSessionDescription(sdp, 'offer'),
    );
    final answer = await _peerConnection!.createAnswer();
    await _peerConnection!.setLocalDescription(answer);

    await _hubConnection.invoke(
      "SendAnswer",
      args: <Object>[
        widget.currentUserId,
        fromUserId,
        answer.sdp ?? "", // ✅ đảm bảo không null
      ],
    );

    setState(() => inCall = true);
  }

  Future<void> _hangUp() async {
    await _peerConnection?.close();
    await _localStream?.dispose();
    _peerConnection = null;
    _localRenderer.srcObject = null;
    _remoteRenderer.srcObject = null;
    setState(() => inCall = false);
  }

  @override
  void dispose() {
    _localRenderer.dispose();
    _remoteRenderer.dispose();
    _hubConnection.stop();
    _peerConnection?.dispose();
    _localStream?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.type == 'video' ? "Video Call" : "Audio Call"),
        actions: [
          if (inCall)
            IconButton(
              icon: const Icon(Icons.call_end),
              color: Colors.red,
              onPressed: _hangUp,
            ),
        ],
      ),
      body: Stack(
        children: [
          Positioned.fill(
            child: widget.type == 'video'
                ? RTCVideoView(_remoteRenderer)
                : const Center(child: Icon(Icons.call, size: 100)),
          ),
          if (widget.type == 'video' && _localStream != null)
            Positioned(
              top: 20,
              right: 20,
              width: 120,
              height: 160,
              child: RTCVideoView(_localRenderer, mirror: true),
            ),
          if (!inCall)
            Center(
              child: ElevatedButton(
                onPressed: _startCall,
                child: const Text("Gọi"),
              ),
            ),
        ],
      ),
    );
  }
}
