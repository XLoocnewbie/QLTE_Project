package com.example.frontend_qlte

import android.content.Intent
import android.provider.Settings
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel

class MainActivity : FlutterActivity() {

    private val CHANNEL = "com.frontend_qlte/restriction"

    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)

        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, CHANNEL)
            .setMethodCallHandler { call, result ->
                when (call.method) {

                    // 🔹 1️⃣ Cập nhật danh sách app bị chặn
                    "updateBlockedApps" -> {
                        val apps = call.argument<List<String>>("apps")
                        if (apps != null) {
                            MyAccessibilityService.blockedApps = apps
                            result.success("Updated blocked apps: ${apps.size}")
                        } else {
                            result.error("INVALID", "App list is null", null)
                        }
                    }

                    // 🔹 2️⃣ Mở trang cài đặt Trợ năng (Accessibility)
                    "openAccessibilitySettings" -> {
                        val intent = Intent(Settings.ACTION_ACCESSIBILITY_SETTINGS)
                        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
                        startActivity(intent)
                        result.success(null)
                    }

                    // 🔹 3️⃣ Kiểm tra xem quyền Trợ năng đã bật chưa
                    "isAccessibilityEnabled" -> {
                        val enabled = Settings.Secure.getString(
                            contentResolver,
                            Settings.Secure.ENABLED_ACCESSIBILITY_SERVICES
                        )?.contains(packageName) == true
                        result.success(enabled)
                    }

                    // 🔹 4️⃣ Mở trang “Hiển thị trên ứng dụng khác” (Overlay permission)
                    "openOverlaySettings" -> {
                        val intent = Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION)
                        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
                        startActivity(intent)
                        result.success(null)
                    }

                    else -> result.notImplemented()
                }
            }
    }
}
