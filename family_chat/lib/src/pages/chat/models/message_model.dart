class MessageModel {
  String messageText;
  int senderId;
  DateTime sentTime;

  MessageModel({
    required this.messageText,
    required this.senderId,
    required this.sentTime,
  });

  Map<String, dynamic> toJson() {
    return {
      'messageText': messageText,
      'senderId': senderId,
      'sentTime': sentTime,
    };
  }

  factory MessageModel.fromJson(Map<String, dynamic> json) {
    return MessageModel(
      messageText: json['messageText'],
      senderId: json['senderId'],
      sentTime: json['sentTime'],
    );
  }
}
