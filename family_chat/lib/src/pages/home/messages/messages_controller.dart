import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import '../../../global_service.dart';
import 'models/user_with_last_message.dart';

class MessagesController with ChangeNotifier {
  final client = http.Client();
  final globalService = GlobalService();

  Future<List<UserWithLastMessage>> getUsersWithLastMessage(int userId, Function(String error) onError) async {
    var url = await globalService.getServerUri('/users-with-last-message/$userId');
    http.Response response;
    try {
      response = await client.get(url);
      // response = await client.get(url, headers: {
      //   HttpHeaders.authorizationHeader: 'Bearer 2',
      // });

      if (response.statusCode != 200) {
        throw Exception('Failed to get users');
      }

      var result = json.decode(response.body)['usersWithLastMessage'].map<UserWithLastMessage>((data) {
        var item = UserWithLastMessage.fromJson(data);
        return item;
      }).toList();

      return result;
    } catch (e) {
      onError.call(e.toString());
      return List.empty();
    }
  }

  @override
  void dispose() {
    client.close();
    super.dispose();
  }
}
