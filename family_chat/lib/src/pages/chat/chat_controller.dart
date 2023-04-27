// ignore_for_file: avoid_print

import 'dart:convert';

import 'package:dart_json_mapper/dart_json_mapper.dart';
import 'package:family_chat/src/pages/home/account/models/user_model.dart';
import 'package:family_chat/src/pages/chat/services/signalr_service.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../home/settings/models/settings_constants.dart';
import 'chat_view_model.dart';
import 'models/message_model.dart';
import 'models/message_response.dart';
import 'models/message_from_to.dart';

class ChatController with ChangeNotifier {
  final client = http.Client();
  late SignalrService? signalrService;
  final MyFunction<List<Object?>> onMessageReceived;

  ChatViewModel viewModel;
  ChatController({required this.viewModel, required this.onMessageReceived}) {
    signalrService = SignalrService(onMessageReceived: onMessageReceived);
  }

  Future<UserModel> getUser(int id) async {
    var url = await getUri('/getuser/$id');

    var response = await client.get(url);

    if (response.statusCode != 200) {
      throw Exception('Failed to load user');
    }
    var user = UserModel.fromJson(jsonDecode(response.body));

    return user;
  }

  Future<MessageModel?> sendMessage(String messageText) async {
    if (messageText.isEmpty) return null;

    var messageModel = MessageModel(
      messageText: messageText,
      senderId: viewModel.myUserId,
      sentTime: DateTime.now(),
    );

    var sender = MessageFromTo();
    sender.from = viewModel.myUserId;
    sender.to = viewModel.userIdChatWith;

    await signalrService!.connection!.invoke("SendMessage", args: [
      sender.toJson(),
      messageText,
    ]).catchError((error) {
      viewModel.showError("Message sending error!");
      return null;
    });

    return messageModel;
  }

  Future<List<MessageModel>> getMessages(Function? onError) async {
    var numberOfMessages = 20;
    var url = await getUri('/get-direct-messages/${viewModel.myUserId}/${viewModel.userIdChatWith}/$numberOfMessages');

    http.Response response;
    try {
      response = await client.get(url);
    } catch (ex) {
      onError?.call();
      return List.empty();
    }

    var messageResponse = JsonMapper.deserialize<MessageResponse>(response.body);

    var messages = <MessageModel>[];

    for (var element in messageResponse?.messages ?? List.empty()) {
      var item = MessageModel(
        messageText: element.text,
        senderId: element.sender,
        sentTime: element.sendTime,
      );
      messages.add(item);
    }

    return messages;
  }

  // connectToMessageHub() async {
  //   await signalrService.connect(() => viewModel.showError("Message hub connection error!"));
  // }

  Future<Uri> getUri(String path) async {
    final prefs = await SharedPreferences.getInstance();

    var protocol = prefs.getString(SettingsConstants.protocol);
    var host = prefs.getString(SettingsConstants.host);
    var port = prefs.getInt(SettingsConstants.port);
    //var baseUrl = '$protocol://$host:$port';

    return Uri(scheme: protocol, host: host, port: port, path: path);
  }
  // Future<String> _getDeviceName() async {
  //   DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();

  //   if (Platform.isAndroid) {
  //     var androidInfo = await deviceInfo.androidInfo;
  //     return androidInfo.device.toString();
  //   } else if (Platform.isIOS) {
  //     var iosInfo = await deviceInfo.iosInfo;
  //     return iosInfo.name.toString();
  //   } else if (Platform.isWindows) {
  //     var windowsInfo = await deviceInfo.windowsInfo;
  //     return windowsInfo.computerName;
  //   }

  //   throw Exception("Platform not supported");
  // }

  @override
  void dispose() {
    super.dispose();
    client.close();
    signalrService?.connection?.stop();
    signalrService = null;
  }
}
