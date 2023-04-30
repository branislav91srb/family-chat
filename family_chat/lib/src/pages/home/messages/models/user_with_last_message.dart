import 'base_message_model.dart';
import 'base_user_model.dart';

class UserWithLastMessage {
  BaseUserModel user;
  BaseMessageModel? message;

  UserWithLastMessage({required this.user, required this.message});

  Map<String, dynamic> toJson() {
    return {
      'user': user.toJson(),
      'message': message?.toJson(),
    };
  }

  factory UserWithLastMessage.fromJson(Map<String, dynamic> json) {
    return UserWithLastMessage(
      user: BaseUserModel.fromJson(json['user']),
      message: json['message'] != null ? BaseMessageModel?.fromJson(json['message']) : null,
    );
  }
}
