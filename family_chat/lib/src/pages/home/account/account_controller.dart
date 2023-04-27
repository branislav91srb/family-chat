import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../settings/models/settings_constants.dart';
import 'models/user_model.dart';

class AccountController with ChangeNotifier {
  final client = http.Client();
  //static const String _baseUrl = '10.0.2.2:5125';

  Future<int> saveUser(UserModel user) async {
    var url = await getUri('/create-user');

    if (user.id != null && user.id! > 0) {
      url = await getUri('/saveuser');
    }

    http.Response response = await client.post(
      url,
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: jsonEncode(user),
    );

    if (response.statusCode == 200) {
      return int.parse(response.body);
    } else {
      throw Exception('Failed to ${(user.id ?? 0) > 0 ? 'Save' : 'Register'} user');
    }
  }

  Future<UserModel> login(String username, String password) async {
    var url = await getUri('/login');
    http.Response response = await client.post(
      url,
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: jsonEncode({'userName': username, 'password': password}),
    );

    if (response.statusCode == 200) {
      return UserModel.fromJson(jsonDecode(response.body));
    } else {
      throw Exception('Failed to login');
    }
  }

  Future<UserModel> getUser(int userId) async {
    var url = await getUri('/getuser/$userId');
    http.Response response = await client.get(url);

    if (response.statusCode == 200) {
      return UserModel.fromJson(jsonDecode(response.body));
    } else {
      throw Exception('Failed to get user');
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
