import 'dart:io';

import 'package:bitsdojo_window/bitsdojo_window.dart';
import 'package:family_chat/src/routes.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:system_tray/system_tray.dart';
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

  await initSystemTray();

  // Run the app and pass in the SettingsController. The app listens to the
  // SettingsController for changes, then passes it further down to the
  // SettingsView.
  runApp(MyApp(
    settingsController: settingsController,
    initialRoute: initialRoute,
  ));

  if (Platform.isWindows) {
    doWhenWindowReady(() {
      final win = appWindow;
      const initialSize = Size(800, 600);
      win.minSize = initialSize;
      win.size = initialSize;
      win.alignment = Alignment.center;
      win.title = "Family Chat";
      win.hide();
    });
  }
}

Future<void> initSystemTray() async {
  if (!Platform.isWindows) return;

  String path = 'assets/images/flutter_logo.ico';

  final AppWindow appWindow = AppWindow();
  final SystemTray systemTray = SystemTray();

  // We first init the systray menu
  await systemTray.initSystemTray(
    title: "system tray",
    iconPath: path,
  );

  // create context menu
  final Menu menu = Menu();
  await menu.buildFrom([
    MenuItemLabel(label: 'Show', onClicked: (menuItem) => appWindow.show()),
    MenuItemLabel(label: 'Hide', onClicked: (menuItem) => appWindow.hide()),
    MenuItemLabel(label: 'Exit', onClicked: (menuItem) => appWindow.close()),
  ]);

  // set context menu
  await systemTray.setContextMenu(menu);

  // handle system tray event
  systemTray.registerSystemTrayEventHandler((eventName) {
    debugPrint("eventName: $eventName");
    if (eventName == kSystemTrayEventClick) {
      Platform.isWindows ? appWindow.show() : systemTray.popUpContextMenu();
    } else if (eventName == kSystemTrayEventRightClick) {
      Platform.isWindows ? systemTray.popUpContextMenu() : appWindow.show();
    }
  });
}
