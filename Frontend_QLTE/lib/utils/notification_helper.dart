// import 'package:flutter_local_notifications/flutter_local_notifications.dart';
// import 'package:timezone/data/latest.dart' as tz;
// import 'package:timezone/timezone.dart' as tz;
//
// class NotificationHelper {
//   static final FlutterLocalNotificationsPlugin _notificationsPlugin =
//       FlutterLocalNotificationsPlugin();
//
//   /// Khởi tạo notification
//   static Future<void> init() async {
//     const AndroidInitializationSettings androidInit =
//         AndroidInitializationSettings('@mipmap/ic_launcher');
//
//     const InitializationSettings settings =
//         InitializationSettings(android: androidInit);
//
//     await _notificationsPlugin.initialize(settings);
//     tz.initializeTimeZones();
//   }
//
//   /// 🕐 Đặt lịch nhắc tự động
//   static Future<void> scheduleNotification({
//     required int id,
//     required String title,
//     required String body,
//     required DateTime dateTime,
//   }) async {
//     await _notificationsPlugin.zonedSchedule(
//       id,
//       title,
//       body,
//       tz.TZDateTime.from(dateTime, tz.local),
//       const NotificationDetails(
//         android: AndroidNotificationDetails(
//           'vaccination_channel',
//           'Lịch tiêm phòng',
//           channelDescription: 'Nhắc lịch tiêm phòng thú cưng',
//           importance: Importance.max,
//           priority: Priority.high,
//         ),
//       ),
//       androidScheduleMode: AndroidScheduleMode.exactAllowWhileIdle,
//       matchDateTimeComponents: DateTimeComponents.dateAndTime,
//     );
//   }
// }
