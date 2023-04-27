class UserModel {
  final int? id;
  final String userName;
  final String password;
  final String avatar;

  UserModel({
    this.id,
    required this.userName,
    required this.password,
    required this.avatar,
  });

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userName': userName,
      'password': password,
      'avatar': avatar,
    };
  }

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      id: json['id'],
      userName: json['userName'],
      password: json['password'],
      avatar: json['avatar'],
    );
  }
}
