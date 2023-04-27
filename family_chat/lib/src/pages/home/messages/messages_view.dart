import 'package:family_chat/src/custom_page.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../routes.dart';
import '../account/models/user_model.dart';
import 'messages_controller.dart';

class MessagesView extends StatefulWidget implements CustomPage {
  const MessagesView({super.key, required this.goToPage});

  final Function goToPage;

  @override
  State<StatefulWidget> createState() => _MessagesViewState();

  @override
  String get pageName => "Messages";
}

/// Displays a list of SampleItems.
class _MessagesViewState extends State<MessagesView> {
  var messagesController = MessagesController();
  late SharedPreferences prefs;
  late int _userId = 0;
  List<UserModel> items = [];

  @override
  void initState() {
    super.initState();

    messagesController.getUsers().then((value) {
      setState(() {
        items = value;
      });
    });
  }

  _init() async {
    prefs = await SharedPreferences.getInstance();
    var userId = prefs.getInt('userId');

    setState(() {
      _userId = userId?.toInt() ?? 0;
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _init(),
      builder: (context, snapshot) {
        return Center(
          // To work with lists that may contain a large number of items, it’s best
          // to use the ListView.builder constructor.
          //
          // In contrast to the default ListView constructor, which requires
          // building all Widgets up front, the ListView.builder constructor lazily
          // builds Widgets as they’re scrolled into view.
          child: ListView.builder(
            // Providing a restorationId allows the ListView to restore the
            // scroll position when a user leaves and returns to the app after it
            // has been killed while running in the background.
            restorationId: 'sampleItemListView',
            itemCount: items.length,
            itemBuilder: (BuildContext context, int index) {
              final item = items[index];

              bool isMe = item.id == _userId;

              return ListTile(
                  title: Opacity(
                    opacity: isMe ? 0.5 : 1.0,
                    child: Text(item.userName),
                  ),
                  leading: Opacity(
                    opacity: isMe ? 0.5 : 1.0,
                    child: CircleAvatar(
                      // Display the Flutter Logo image asset.
                      foregroundImage: NetworkImage(item.avatar),
                    ),
                  ),
                  onTap: () {
                    // Navigate to the details page. If the user leaves and returns to
                    // the app after it has been killed while running in the
                    // background, the navigation stack is restored.
                    // Navigator.restorablePushNamed(
                    //   context,
                    //   SampleItemDetailsView.routeName,
                    // );

                    if (isMe) return;

                    Navigator.restorablePushNamed(
                      context,
                      Routes.chat,
                      arguments: item.id,
                    );
                  });
            },
          ),
        );
      },
    );
  }
}
