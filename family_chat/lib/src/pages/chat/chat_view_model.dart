import 'package:flutter/material.dart';

import '../home/account/models/user_model.dart';
import 'models/message_model.dart';

class ChatViewModel extends ChangeNotifier {
  final textController = TextEditingController();

  List<MessageModel> messages = List.empty(growable: true);
  //   List.generate(
  //   20,
  //   (i) => MessageModel(
  //       messageText: "Message text $i",
  //       senderAvatar: 'https://www.w3schools.com/howto/img_avatar.png',
  //       messageType: i % 3 == 0 ? MessageSendType.sent : MessageSendType.received,
  //       sentTime: DateTime.now()),
  // );

  String errorMessage = "";
  bool showErrorMessage = false;
  int myUserId;
  String myUserAvatar = "";
  int userIdChatWith;
  String userNameChatWith = "";
  String userAvatarChatWith = "";

  ChatViewModel(this.userIdChatWith, this.myUserId);

  void addMessage(MessageModel message) {
    messages.insert(messages.length, message);
    notifyListeners();
  }

  void addMessages(List<MessageModel> newMessages) {
    for (var message in newMessages) {
      messages.insert(messages.length, message);
    }
    notifyListeners();
  }

  void showError(String message) {
    errorMessage = message;
    showErrorMessage = true;
    notifyListeners();
  }

  void hideError() {
    showErrorMessage = false;
    errorMessage = "";
    notifyListeners();
  }

  void setUserChatWith(UserModel user) {
    userNameChatWith = user.userName;
    userAvatarChatWith = user.avatar;
    notifyListeners();
  }

  void setMyUser(UserModel user) {
    myUserAvatar = user.avatar;
    notifyListeners();
  }

  @override
  void dispose() {
    super.dispose();
    textController.dispose();
  }
}
