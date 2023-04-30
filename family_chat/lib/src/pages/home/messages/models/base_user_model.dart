class BaseUserModel {
  final int id;
  final String userName;
  final String avatar;
  bool isOnline = false;

  BaseUserModel({
    required this.id,
    required this.userName,
    required this.avatar,
    this.isOnline = false,
  });

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userName': userName,
      'avatar': avatar,
      'isOnline': isOnline,
    };
  }

  factory BaseUserModel.fromJson(Map<String, dynamic> json) {
    return BaseUserModel(
      id: json['id'],
      userName: json['userName'],
      avatar: json['avatar'],
      isOnline: json['isOnline'] ?? false,
    );
  }
}
