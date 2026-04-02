import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:frontend_qlte/screens/admin/admin_children_screen.dart';
import 'package:frontend_qlte/screens/admin/admin_parent_children_screen.dart';
import 'package:frontend_qlte/screens/admin/admin_refresh_token_screen.dart';
import 'package:frontend_qlte/screens/admin/admin_user_screen.dart';
import 'package:frontend_qlte/screens/admin/admin_home_screen.dart';
import 'package:frontend_qlte/screens/auth/login_screen.dart';
import 'package:frontend_qlte/screens/auth/registration_screen.dart';
import 'package:frontend_qlte/screens/client/chat_message_list_screen.dart';
import 'package:frontend_qlte/screens/client/child/exam_schedule_children_screen.dart';
import 'package:frontend_qlte/screens/client/child/home_children_screen.dart';
import 'package:frontend_qlte/screens/client/child/schedule_children_screen.dart';
import 'package:frontend_qlte/screens/client/child/sos_request_children_screen.dart';
import 'package:frontend_qlte/screens/client/children_manager_screen.dart';
import 'package:frontend_qlte/screens/client/device_info_child_screen.dart';
import 'package:frontend_qlte/screens/client/home_screen.dart';
import 'package:frontend_qlte/screens/client/restriction_parent_screen.dart';
import 'package:frontend_qlte/screens/client/schedule_parent_screen.dart';
import 'package:frontend_qlte/screens/client/sos_request_parent_screen.dart';
import 'package:frontend_qlte/screens/client/study_period_parent_screen.dart';
import 'package:frontend_qlte/screens/client/user_infor_screen.dart';
import 'package:frontend_qlte/screens/common/navigation_bar_child_screen.dart';
import 'package:frontend_qlte/screens/common/navigation_bar_screen.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await dotenv.load(fileName: ".env");
  print("✅ BASE_URL loaded = ${dotenv.env['BASE_URL']}");

  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Ứng dụng quản lý trẻ em',
      debugShowCheckedModeBanner: false,
      navigatorKey: navigatorKey,
      theme: ThemeData(
        primarySwatch: Colors.blue,
        useMaterial3: false,
      ),
      initialRoute: '/login',

      // 🧭 Các route tĩnh
      routes: {
        '/login': (context) => const LoginScreen(),
        '/home': (context) => const HomeScreen(),
        '/admin': (context) => const AdminHomeScreen(),
        '/admin_user': (context) => const AdminUserScreen(),
        '/register': (context) => const RegistrationScreen(),
        '/chatmassagelist': (context) => const ChatMessageListScreen(),
        '/navigationbar': (context) => const NavigationBarScreen(),
        '/navigationbarchild': (context) => const NavigationBarChildScreen(),
        '/child': (context) => const HomeChildrenScreen(),
        '/schedule_children': (context) => const ScheduleChildrenScreen(),
        '/exam_schedule_children': (context) => const ExamScheduleChildrenScreen(),
        '/sos_request_children': (context) => const SOSRequestChildrenScreen(),
        '/sos_request_parent': (context) => const SOSRequestParentScreen(sosData: {}),
        '/device_info_child': (context) => const DeviceInfoChildScreen(),
        '/study_period_parent': (context) => const StudyPeriodParentScreen(),
        '/restriction_parent': (context) => const RestrictionParentScreen(),
        '/schedule_parent': (context) => const ScheduleParentScreen(),
        '/userinfo': (context) => const UserInforScreen(),
        '/child_manager': (context) => const ChildrenManagerScreen(),
        '/admin_refresh_token': (context) => const AdminRefreshTokenScreen(),
      },

      // 🧩 Các route động (có arguments)
      onGenerateRoute: (settings) {
        switch (settings.name) {
        // 🟢 Admin xem danh sách parent
          case '/admin_children':
            final args = settings.arguments as Map<String, dynamic>;
            return MaterialPageRoute(
              builder: (_) => AdminChildrenScreen(
                token: args['token'],
              ),
            );

        // 🟣 Admin xem danh sách child của parent
          case '/admin_parent_children':
            final args = settings.arguments as Map<String, dynamic>;
            return MaterialPageRoute(
              builder: (_) => AdminParentChildrenScreen(
                token: args['token'],
                parentId: args['parentId'],
                parentName: args['parentName'],
              ),
            );

        }
      },
    );
  }
}
