class MessageModel {
  String messageText;
  int senderId;
  DateTime sentTime;

  MessageModel({
    required this.messageText,
    required this.senderId,
    required this.sentTime,
  });
}

// enum MessageSendType { sent, received }
