import 'package:flutter_dotenv/flutter_dotenv.dart';

class Config_URL {
  static final String urlServer = (() {
    String? url = dotenv.env['URL_SERVER'];

    if (url == null || url.isEmpty) {
      print("⚠️ URL_SERVER is not set in the .env file. Using default URL.");
      url = "https://firstsagecat76.conveyor.cloud/api/";
    }

    // 🔧 Đảm bảo có dấu '/' ở cuối để tránh lỗi nối chuỗi
    if (!url.endsWith("/")) {
      url = "$url/";
    }

    print("✅ URL_SERVER Loaded once: $url");
    return url;
  })();

  static final String baseUrl = (() {
    String? url = dotenv.env['BASE_URL'];

    if (url == null || url.isEmpty) {
      print("⚠️ BASE_URL is not set in the .env file. Using default URL.");
      url = "https://lasttealshed54.conveyor.cloud/api/";
    }

    if (!url.endsWith("/")) {
      url = "$url/";
    }

    print("✅ BASE_URL Loaded once: $url");
    return url;
  })();

  // 🔑 Google Client ID (dành cho ứng dụng Android / iOS)
  static final String clientIdGoogle = (() {
    String? id = dotenv.env['CLIENT_ID_GOOGLE'];

    if (id == null || id.isEmpty) {
      print(
        "⚠️ CLIENT_ID_GOOGLE is not set in the .env file. Using default ID.",
      );
      id ="645483323057-a6ghml2q50j81q3n63ovgadm8u96m5k8.apps.googleusercontent.com";
    }

    print("✅ CLIENT_ID_GOOGLE Loaded once: $id");
    return id;
  })();

  // 🖥️ Google Server Client ID (dành cho backend verification)
  static final String serverClientIdGoogle = (() {
    String? id = dotenv.env['SERVER_CLIENT_ID_GOOGLE'];

    if (id == null || id.isEmpty) {
      print(
        "⚠️ SERVER_CLIENT_ID_GOOGLE is not set in the .env file. Using default ID.",
      );
      id ="645483323057-c6q3bkj13crooeo2mdofa271shf2h17s.apps.googleusercontent.com";
    }

    print("✅ SERVER_CLIENT_ID_GOOGLE Loaded once: $id");
    return id;
  })();
}
