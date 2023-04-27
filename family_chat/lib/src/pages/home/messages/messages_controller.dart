import 'dart:convert';

import 'package:family_chat/src/pages/home/settings/models/settings_constants.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../account/models/user_model.dart';

class MessagesController with ChangeNotifier {
  final client = http.Client();
  // static const String _baseUrl = '10.0.2.2:5125';

  Future<List<UserModel>> getUsers() async {
    var url = await getUri('/getusers');
    http.Response response = await client.get(url);

    if (response.statusCode == 200) {
      var result = json.decode(response.body).map<UserModel>((data) {
        var model = UserModel.fromJson(data);
        return model;
      }).toList();

      return result;
    } else {
      throw Exception('Failed to get users');
    }
  }

  Future<Uri> getUri(String path) async {
    final prefs = await SharedPreferences.getInstance();

    var protocol = prefs.getString(SettingsConstants.protocol);
    var host = prefs.getString(SettingsConstants.host);
    var port = prefs.getInt(SettingsConstants.port);
    //var baseUrl = '$protocol://$host:$port';

    return Uri(scheme: protocol, host: host, port: port, path: path);
  }

  @override
  void dispose() {
    client.close();
    super.dispose();
  }
}
