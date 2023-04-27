class GlobalService {
  static final GlobalService _singleton = GlobalService._internal();

  factory GlobalService() {
    return _singleton;
  }

  GlobalService._internal();

  int userId = 0;

  // int? _userId;
  // int get userId => _userId ?? 0;
  // set userId(int value) => userId = value;
}
