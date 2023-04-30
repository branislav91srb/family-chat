class BaseMessageModel {
  String text;
  int from;
  int to;
  DateTime sendTime;

  BaseMessageModel({
    required this.text,
    required this.from,
    required this.to,
    required this.sendTime,
  });

  Map<String, dynamic> toJson() {
    return {
      'text': text,
      'from': from,
      'to': to,
      'sendTime': sendTime,
    };
  }

  factory BaseMessageModel.fromJson(Map<String, dynamic> json) {
    return BaseMessageModel(
      text: json['text'],
      from: json['from'],
      to: json['to'],
      sendTime: DateTime.parse(json['sendTime']),
    );
  }
}
