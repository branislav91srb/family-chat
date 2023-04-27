import 'package:family_chat/src/routes.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'main.mapper.g.dart' show initializeJsonMapper;
import 'src/app.dart';
import 'src/pages/theme_settings/theme_settings_controller.dart';
import 'src/pages/theme_settings/theme_settings_service.dart';

void main() async {
  initializeJsonMapper();

  WidgetsFlutterBinding.ensureInitialized();

  // init a shared preferences variable
  final SharedPreferences prefs = await SharedPreferences.getInstance();
  var userId = prefs.getInt('userId') ?? 0;

  // define the initial route based on whether the user is logged in or not
  String initialRoute = userId > 0 ? "${Routes.home}/0" : '${Routes.home}/1';

  // Set up the SettingsController, which will glue user settings to multiple
  // Flutter Widgets.
  final settingsController = ThemeSettingsController(ThemeSettingsService());

  // Load the user's preferred theme while the splash screen is displayed.
  // This prevents a sudden theme change when the app is first displayed.
  await settingsController.loadSettings();

  // Run the app and pass in the SettingsController. The app listens to the
  // SettingsController for changes, then passes it further down to the
  // SettingsView.
  runApp(MyApp(
    settingsController: settingsController,
    initialRoute: initialRoute,
  ));
}
