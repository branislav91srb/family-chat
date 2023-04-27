class MessageFromTo {
  late int from;
  late int to;

  Map<String, dynamic> toJson() {
    return {'from': from, 'to': to};
  }
}
