import 'package:family_chat/src/pages/chat/chat_view.dart';
import 'package:family_chat/src/pages/home/home_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:flutter_localizations/flutter_localizations.dart';

import 'routes.dart';
import 'pages/theme_settings/theme_settings_controller.dart';
import 'pages/theme_settings/theme_settings_view.dart';

/// The Widget that configures your application.
class MyApp extends StatelessWidget {
  const MyApp({super.key, required this.settingsController, required this.initialRoute});

  final ThemeSettingsController settingsController;
  final String initialRoute;

  @override
  Widget build(BuildContext context) {
    // Glue the SettingsController to the MaterialApp.
    //
    // The AnimatedBuilder Widget listens to the SettingsController for changes.
    // Whenever the user updates their settings, the MaterialApp is rebuilt.
    return AnimatedBuilder(
      animation: settingsController,
      builder: (BuildContext context, Widget? child) {
        return MaterialApp(
          initialRoute: initialRoute,

          // Providing a restorationScopeId allows the Navigator built by the
          // MaterialApp to restore the navigation stack when a user leaves and
          // returns to the app after it has been killed while running in the
          // background.
          restorationScopeId: 'app',

          // Provide the generated AppLocalizations to the MaterialApp. This
          // allows descendant Widgets to display the correct translations
          // depending on the user's locale.
          localizationsDelegates: const [
            AppLocalizations.delegate,
            GlobalMaterialLocalizations.delegate,
            GlobalWidgetsLocalizations.delegate,
            GlobalCupertinoLocalizations.delegate,
          ],
          supportedLocales: const [
            Locale('en', ''), // English, no country code
          ],

          // Use AppLocalizations to configure the correct application title
          // depending on the user's locale.
          //
          // The appTitle is defined in .arb files found in the localization
          // directory.
          onGenerateTitle: (BuildContext context) => AppLocalizations.of(context)!.appTitle,

          theme: ThemeData(
            //useMaterial3: true,
            textSelectionTheme: const TextSelectionThemeData(
              selectionColor: Colors.green,
              selectionHandleColor: Colors.white70,
            ),
          ),
          //darkTheme: ThemeData.dark(),
          darkTheme: ThemeData(
            //useMaterial3: true,
            brightness: Brightness.dark,
            textSelectionTheme: const TextSelectionThemeData(
              selectionColor: Colors.green,
              selectionHandleColor: Colors.white70,
            ),
          ),
          themeMode: settingsController.themeMode,

          // Define a function to handle named routes in order to support
          // Flutter web url navigation and deep linking.
          onGenerateRoute: (RouteSettings routeSettings) {
            return MaterialPageRoute<void>(
              settings: routeSettings,
              builder: (BuildContext context) => _createRoutes(routeSettings),
            );
          },
        );
      },
    );
  }

  Widget _createRoutes(RouteSettings routeSettings) {
    if (routeSettings.name == Routes.themeSettings) {
      return ThemeSettingsView(controller: settingsController);
    } else if (routeSettings.name == Routes.chat) {
      return ChatView(
        userIdChatWith: routeSettings.arguments as int,
      );
    } else if (routeSettings.name!.startsWith("/home")) {
      var pageIndex = int.tryParse(routeSettings.name!.split("/").last) ?? 0;
      return HomeView(pageIndex: pageIndex);
    } else {
      return const HomeView(pageIndex: 0);
    }
  }
}
