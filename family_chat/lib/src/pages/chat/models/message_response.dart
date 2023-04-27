import 'package:dart_json_mapper/dart_json_mapper.dart';

@jsonSerializable
class MessageResponse {
  List<MessageResponseItem> messages;

  MessageResponse({required this.messages});
}

@jsonSerializable
class MessageResponseItem {
  final String text;
  final int sender;
  final int messageType;
  final DateTime sendTime;

  MessageResponseItem({
    required this.text,
    required this.sender,
    required this.messageType,
    required this.sendTime,
  });
}

// @jsonSerializable
// class MessageSenderResponse {
//   final String userName;
//   final String avatar;

//   MessageSenderResponse({required this.userName, required this.avatar});
// }
