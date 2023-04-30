import 'package:family_chat/src/pages/home/settings/models/settings_constants.dart';
import 'package:shared_preferences/shared_preferences.dart';

class GlobalService {
  Future<Uri> getServerUri([String? path]) async {
    final prefs = await SharedPreferences.getInstance();

    var protocol = prefs.getString(SettingsConstants.protocol);
    var host = prefs.getString(SettingsConstants.host);
    var port = prefs.getInt(SettingsConstants.port);

    if (path == null) {
      return Uri(scheme: protocol, host: host, port: port);
    }
    return Uri(scheme: protocol, host: host, port: port, path: path);
  }
}
