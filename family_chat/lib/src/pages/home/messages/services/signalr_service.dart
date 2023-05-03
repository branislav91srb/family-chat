import 'package:family_chat/src/global_service.dart';
import 'package:flutter/widgets.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'package:signalr_core/signalr_core.dart';

typedef MyFunction<T> = void Function(List<Object?>?);

class SignalrService {
  final GlobalService globalService = GlobalService();
  static const String messageReceiveEventName = 'ReceiveMessage';
  HubConnection? connection;

  final Map<String, MyFunction<List<Object?>>> events;

  SignalrService({required this.events}) {
    globalService.getServerUri('/chatHub').then((value) async {
      connection = HubConnectionBuilder()
          .withUrl(
              value.toString(),
              HttpConnectionOptions(
                logging: (level, message) => debugPrint(message),
                accessTokenFactory: () async => await createAuthToken(),
              ))
          .build();

      await connection!.start();

      events.forEach((eventName, eventFunction) {
        connection!.on(eventName, (message) {
          debugPrint(eventFunction.toString());
          eventFunction(message);
        });
      });
    });
  }

  Future<String> createAuthToken() async {
    final prefs = await SharedPreferences.getInstance();
    var userId = prefs.getInt('userId')!;
    return "Bearer $userId";
  }
}
