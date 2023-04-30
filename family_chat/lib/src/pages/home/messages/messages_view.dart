import 'package:audioplayers/audioplayers.dart';
import 'package:badges/badges.dart';
import 'package:family_chat/src/custom_page.dart';
import 'package:family_chat/src/custom_widgets/error_message.dart';
import 'package:family_chat/src/pages/home/messages/services/signalr_service.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:badges/badges.dart' as badges;

import '../../../routes.dart';
import 'messages_controller.dart';
import 'models/user_with_last_message.dart';

class MessagesView extends StatefulWidget implements CustomPage {
  const MessagesView({super.key, required this.goToPage});

  final Function goToPage;

  @override
  State<StatefulWidget> createState() => _MessagesViewState();

  @override
  String get pageName => "Messages";
}

class _MessagesViewState extends State<MessagesView> {
  SignalrService? signalrService;
  final player = AudioPlayer();
  var messagesController = MessagesController();

  bool loadInterface = false;
  late SharedPreferences prefs;
  late int _userId = 0;
  List<UserWithLastMessage> allUsers = [];
  String _errorMessage = "";
  final Map<int, int> _unreadMessages = {};

  @override
  void initState() {
    super.initState();

    _init().then((value) {
      setState(() {
        loadInterface = true;
      });
    });
  }

  _init() async {
    prefs = await SharedPreferences.getInstance();
    var userId = prefs.getInt('userId');

    setState(() {
      _userId = userId?.toInt() ?? 0;
    });

    if (_userId == 0) {
      return;
    }

    var users = await messagesController.getUsersWithLastMessage(_userId, (error) {
      setState(() {
        _errorMessage = error.toString();
      });
    });

    setState(() {
      for (var user in users) {
        _unreadMessages[user.user.id] = 0;
      }
    });

    setState(() {
      allUsers = users;
    });

    SignalrService(events: {
      "ReceiveMessage": onMessageReceived,
      "UpdateOnlineUsers": onOnlineUsersUpdate,
    });
  }

  @override
  Widget build(BuildContext context) {
    if (!loadInterface) return const Center(child: CircularProgressIndicator());

    return Column(
      children: [
        Expanded(
          child: ListView.builder(
            itemCount: allUsers.length,
            itemBuilder: (BuildContext context, int index) {
              final item = allUsers[index];

              bool isMe = item.user.id == _userId;

              return ListTile(
                  title: Opacity(
                    opacity: isMe ? 0.5 : 1.0,
                    child: Text(item.user.userName),
                  ),
                  leading: Opacity(
                    opacity: isMe ? 0.5 : 1.0,
                    child: Stack(
                      children: [
                        CircleAvatar(
                          // Display the Flutter Logo image asset.
                          foregroundImage: NetworkImage(item.user.avatar),
                        ),
                        Positioned(
                          top: 0,
                          right: 0,
                          child: badges.Badge(
                            showBadge: item.user.isOnline,
                            badgeAnimation: const BadgeAnimation.scale(
                              animationDuration: Duration(milliseconds: 500),
                              curve: Curves.fastOutSlowIn,
                            ),
                          ),
                        )
                      ],
                    ),
                  ),
                  subtitle: Opacity(
                    opacity: isMe ? 0.5 : 1.0,
                    child: Text(
                      item.message?.text ?? "",
                      overflow: TextOverflow.ellipsis,
                      maxLines: 1,
                    ),
                  ),
                  trailing: badges.Badge(
                    showBadge: _unreadMessages[item.user.id]! > 0,
                    badgeContent: Text(_unreadMessages[item.user.id].toString()),
                    badgeAnimation: const BadgeAnimation.scale(
                      animationDuration: Duration(milliseconds: 500),
                      curve: Curves.fastOutSlowIn,
                    ),
                  ),
                  onTap: () {
                    if (isMe) return;

                    setState(() {
                      _unreadMessages[item.user.id] = 0;
                    });

                    Navigator.pushNamed(
                      context,
                      Routes.chat,
                      arguments: item.user.id,
                    );
                  });
            },
          ),
        ),
        SizedBox(
          width: MediaQuery.of(context).size.width * 0.9,
          child: _errorMessage.isNotEmpty
              ? ErrorMessage(message: _errorMessage, closeError: () => setState(() => _errorMessage = ""))
              : null,
        )
      ],
    );
  }

  void onMessageReceived(List<Object?>? params) async {
    var value = params?[0] as Map<String, dynamic>;

    if (value["sender"] == 0) return;

    var senderId = value["sender"] as int;

    await player.play(AssetSource('sounds/notification.wav'));

    setState(() {
      _unreadMessages[senderId] = (_unreadMessages[senderId] ?? 0) + 1;

      for (var item in allUsers) {
        if (item.user.id == senderId) {
          item.message?.text = value["text"];
          break;
        }
      }
    });
  }

  void onOnlineUsersUpdate(List<Object?>? params) {
    if (params == null) return;

    var data = params[0] as List<dynamic>;

    var onlineUsers = List<int>.empty(growable: true);

    for (var item in data) {
      onlineUsers.add(int.parse(item![0]));
    }

    setState(() {
      for (var item in allUsers) {
        item.user.isOnline = onlineUsers.contains(item.user.id);
      }
    });
  }

  @override
  void dispose() async {
    super.dispose();
    player.dispose();
    await signalrService?.connection?.stop();
  }
}
