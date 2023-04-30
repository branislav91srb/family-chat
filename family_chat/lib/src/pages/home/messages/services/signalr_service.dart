import 'package:shared_preferences/shared_preferences.dart';

import 'package:signalr_core/signalr_core.dart';
import '../../settings/models/settings_constants.dart';

typedef MyFunction<T> = void Function(List<Object?>?);

class SignalrService {
  static const String messageReceiveEventName = 'ReceiveMessage';
  HubConnection? connection;

  final MyFunction<List<Object?>> onMessageReceived;

  SignalrService({required this.onMessageReceived}) {
    getUri('/chatHub').then((value) async {
      connection = HubConnectionBuilder()
          .withUrl(
              value.toString(),
              HttpConnectionOptions(
                logging: (level, message) => print(message),
              ))
          .build();

      await connection!.start();

      connection!.on(messageReceiveEventName, (message) {
        print(message.toString());
        onMessageReceived(message);
      });
    });
  }

  Future<Uri> getUri(String path) async {
    final prefs = await SharedPreferences.getInstance();

    var protocol = prefs.getString(SettingsConstants.protocol);
    var host = prefs.getString(SettingsConstants.host);
    var port = prefs.getInt(SettingsConstants.port);

    return Uri(scheme: protocol, host: host, port: port, path: path);
  }
}
