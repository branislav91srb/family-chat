// ignore_for_file: avoid_print

import 'package:audioplayers/audioplayers.dart';
import 'package:family_chat/src/pages/chat/chat_view_model.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'chat_controller.dart';
import 'models/message_model.dart';

class ChatView extends StatefulWidget {
  const ChatView({Key? key, required this.userIdChatWith}) : super(key: key);
  static const routeName = '/chat';
  final int userIdChatWith;

  @override
  State<ChatView> createState() => _ChatViewState();
}

class _ChatViewState extends State<ChatView> {
  late int userIdChatWith;
  int myUserId = 0;
  late ChatViewModel viewModel;
  late ChatController controller;
  late SharedPreferences prefs;
  bool loadInterface = false;

  ScrollController scrollController = ScrollController();
  final DateFormat formatter = DateFormat('dd-MM-yy HH:mm');

  final player = AudioPlayer();

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

    setState(() {
      myUserId = prefs.getInt('userId')!;
      userIdChatWith = widget.userIdChatWith;
    });

    viewModel = ChatViewModel(userIdChatWith, myUserId);
    controller = ChatController(viewModel: viewModel, onMessageReceived: onMessageReceived);
    // await controller.connectToMessageHub();

    var myUser = await controller.getUser(myUserId);
    viewModel.setMyUser(myUser);

    var userChatWith = await controller.getUser(userIdChatWith);
    viewModel.setUserChatWith(userChatWith);

    var messages = await controller.getMessages(() => print("Error while getting messages!"));
    viewModel.addMessages(messages);

    await Future.delayed(const Duration(milliseconds: 700));
  }

  @override
  Widget build(BuildContext context) {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (scrollController.hasClients) {
        scrollController.animateTo(scrollController.position.maxScrollExtent,
            duration: const Duration(milliseconds: 500), curve: Curves.fastOutSlowIn);
      }
    });

    if (!loadInterface) return const Center(child: CircularProgressIndicator());

    return Scaffold(
      appBar: AppBar(
        title: const Text('Chat'),
      ),
      body: Container(
        padding: const EdgeInsets.only(bottom: 10),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            Container(
              decoration: BoxDecoration(
                color: Colors.blue.shade800,
                boxShadow: const [
                  BoxShadow(
                    color: Colors.grey,
                    blurRadius: 4,
                    offset: Offset(0, 3), // Shadow position
                  ),
                ],
              ),
              child: SizedBox(
                height: 30,
                child: Center(
                  child: Text(
                    viewModel.userNameChatWith,
                    style: const TextStyle(color: Colors.white),
                  ),
                ),
              ),
            ),
            Expanded(
              child: ListView.builder(
                  itemCount: viewModel.messages.length,
                  controller: scrollController,
                  itemBuilder: (context, index) {
                    var currentMessage = viewModel.messages[index];

                    if (currentMessage.senderId == viewModel.myUserId) {
                      return sentMessageWidget(currentMessage);
                    } else {
                      return receivedMessageWidget(currentMessage);
                    }
                  }),
            ),
            const Divider(
              height: 20,
              color: Colors.black,
            ),
            SizedBox(
              height: 100,
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 10),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Expanded(
                      child: TextField(
                        controller: viewModel.textController,
                        decoration: const InputDecoration(labelText: 'Write message'),
                      ),
                    ),
                    const SizedBox(
                      width: 20,
                    ),
                    ElevatedButton.icon(
                      onPressed: () async {
                        String text = viewModel.textController.text;
                        var message = await controller.sendMessage(text);
                        viewModel.textController.text = "";

                        if (message == null) return;

                        setState(() {
                          viewModel.addMessage(message);
                        });
                      },
                      icon: const Icon(Icons.send),
                      label: const Text('Send'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.blue,
                        foregroundColor: Colors.white,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            Visibility(
              visible: viewModel.showErrorMessage,
              child: Container(
                padding: const EdgeInsets.all(10.00),
                //margin: const EdgeInsets.only(bottom: 10.00),
                color: Colors.red,
                child: Row(
                  children: [
                    Container(
                      margin: const EdgeInsets.only(right: 6.00),
                      child: const Icon(Icons.info, color: Colors.white),
                    ), // icon for error message
                    Text(viewModel.errorMessage, style: const TextStyle(color: Colors.white)),
                    Expanded(
                      child: Align(
                        alignment: Alignment.centerRight,
                        child: IconButton(
                          icon: const Icon(Icons.close),
                          onPressed: () {
                            viewModel.hideError();
                          },
                        ),
                      ),
                    )
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget sentMessageWidget(currentMessage) {
    return Container(
      padding: const EdgeInsets.all(8.0),
      child: Row(
        children: [
          const SizedBox(width: 30),
          Expanded(
            child: Container(
              padding: const EdgeInsets.only(
                bottom: 15,
                left: 15,
                right: 15,
                top: 3,
              ),
              decoration: BoxDecoration(
                color: Colors.blue,
                border: Border.all(
                  color: Colors.blueGrey,
                ),
                borderRadius: const BorderRadius.all(
                  Radius.circular(10),
                ),
              ),
              child: Stack(
                children: [
                  Container(
                    alignment: Alignment.centerRight,
                    padding: const EdgeInsets.only(top: 8),
                    child: Text(
                      currentMessage.messageText,
                      textAlign: TextAlign.end,
                    ),
                  ),
                  Container(
                    alignment: Alignment.topRight,
                    padding: const EdgeInsets.only(bottom: 5),
                    child: Text(
                      formatter.format(currentMessage.sentTime),
                      textAlign: TextAlign.end,
                      style: TextStyle(
                        fontSize: 6,
                        color: Colors.grey.shade200,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(width: 20),
          CircleAvatar(
            radius: 30,
            backgroundImage: NetworkImage(viewModel.myUserAvatar),
          ),
        ],
      ),
    );
  }

  Widget receivedMessageWidget(MessageModel currentMessage) {
    return Container(
      padding: const EdgeInsets.all(8.0),
      child: Row(
        children: [
          CircleAvatar(
            radius: 30,
            backgroundImage: NetworkImage(viewModel.userAvatarChatWith),
          ),
          const SizedBox(width: 20),
          Expanded(
            child: Container(
              padding: const EdgeInsets.only(
                bottom: 15,
                left: 15,
                right: 15,
                top: 3,
              ),
              decoration: BoxDecoration(
                color: Colors.blue,
                border: Border.all(
                  color: Colors.blueGrey,
                ),
                borderRadius: const BorderRadius.all(
                  Radius.circular(10),
                ),
              ),
              child: Stack(
                children: [
                  Container(
                    alignment: Alignment.centerLeft,
                    padding: const EdgeInsets.only(top: 8),
                    child: Text(
                      currentMessage.messageText,
                      textAlign: TextAlign.end,
                    ),
                  ),
                  Container(
                    alignment: Alignment.topRight,
                    padding: const EdgeInsets.only(bottom: 5),
                    child: Text(
                      formatter.format(currentMessage.sentTime),
                      textAlign: TextAlign.end,
                      style: TextStyle(
                        fontSize: 6,
                        color: Colors.grey.shade200,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(width: 30)
        ],
      ),
    );
  }

  void onMessageReceived(List<Object?>? params) {
    var value = params?[0] as Map<String, dynamic>;

    if (value["sender"] == 0) return;
    if (value["sender"] == viewModel.myUserId) return; // Todo: remove this line

    var messageModel = MessageModel(
      messageText: value['text'],
      senderId: value['sender'],
      sentTime: DateTime.parse(value['sendTime']),
    );

    player.play(AssetSource('sounds/notification.wav'));
    setState(() {
      viewModel.addMessage(messageModel);
    });
  }

  @override
  void dispose() {
    scrollController.dispose();
    super.dispose();
  }
}
