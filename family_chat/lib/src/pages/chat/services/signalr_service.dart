import 'package:shared_preferences/shared_preferences.dart';

import 'package:signalr_core/signalr_core.dart';
import '../../home/settings/models/settings_constants.dart';

typedef MyFunction<T> = void Function(List<Object?>?);

class SignalrService {
  // final serverUrl = "http://192.168.100.100:9001/chatHub";
  //final serverUrl = "http://10.0.2.2:5125/chatHub";
  HubConnection? connection;

  final MyFunction<List<Object?>> onMessageReceived;

  SignalrService({required this.onMessageReceived}) {
    // final httpOptions = HttpConnectionOptions();

    getUri('/chatHub').then((value) async {
      //hubConnection = HubConnectionBuilder().withUrl(value.toString(), options: httpOptions).build();
      connection = HubConnectionBuilder()
          .withUrl(
              value.toString(),
              HttpConnectionOptions(
                logging: (level, message) => print(message),
              ))
          .build();

      await connection!.start();

      connection!.on('ReceiveMessage', (message) {
        print(message.toString());
        onMessageReceived(message);
      });
    });
  }

  // connect(Function? onError) async {
  //   // TODO: remove this delay
  //   await Future.delayed(const Duration(seconds: 2));

  //   if (connection == null) {
  //     throw ArgumentError("Connection is null");
  //   }

  //   // if (hubConnection!.state == HubConnectionState.Connected) {
  //   //   return;
  //   // }

  //   try {
  //     await connection!.start();
  //     connection!.on(
  //       "ReceiveMessage",
  //       (arguments) {
  //         onMessageReceived(arguments);
  //       },
  //     );
  //   } catch (ex) {
  //     onError?.call();
  //   }
  // }

  Future<Uri> getUri(String path) async {
    final prefs = await SharedPreferences.getInstance();

    var protocol = prefs.getString(SettingsConstants.protocol);
    var host = prefs.getString(SettingsConstants.host);
    var port = prefs.getInt(SettingsConstants.port);
    //var baseUrl = '$protocol://$host:$port';

    return Uri(scheme: protocol, host: host, port: port, path: path);
  }
}
