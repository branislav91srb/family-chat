class BaseUserModel {
  final int id;
  final String userName;
  final String avatar;

  BaseUserModel({
    required this.id,
    required this.userName,
    required this.avatar,
  });

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userName': userName,
      'avatar': avatar,
    };
  }

  factory BaseUserModel.fromJson(Map<String, dynamic> json) {
    return BaseUserModel(
      id: json['id'],
      userName: json['userName'],
      avatar: json['avatar'],
    );
  }
}
