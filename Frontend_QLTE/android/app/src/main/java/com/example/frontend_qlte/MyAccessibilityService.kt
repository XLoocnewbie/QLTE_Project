package com.example.frontend_qlte

import android.accessibilityservice.AccessibilityService
import android.view.accessibility.AccessibilityEvent
import android.widget.Toast
import android.content.Intent
import android.util.Log

class MyAccessibilityService : AccessibilityService() {

    companion object {
        var blockedApps: List<String> = emptyList()
    }

    override fun onAccessibilityEvent(event: AccessibilityEvent?) {
        if (event == null) return
        if (event.eventType != AccessibilityEvent.TYPE_WINDOW_STATE_CHANGED) return

        val packageName = event.packageName?.toString() ?: return
        Log.d("RestrictionService", "Foreground app: $packageName")

        // 🛑 Nếu package nằm trong danh sách bị chặn
        if (blockedApps.any { packageName.contains(it, ignoreCase = true) }) {
            Toast.makeText(this, "🚫 Ứng dụng $packageName bị chặn trong giờ học!", Toast.LENGTH_SHORT).show()
            performGlobalAction(GLOBAL_ACTION_HOME)  // ✅ Quay về màn hình chính an toàn
        }
    }

    override fun onInterrupt() {
        Log.d("RestrictionService", "Accessibility Service bị ngắt")
    }
}
