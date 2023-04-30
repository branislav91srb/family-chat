import 'package:family_chat/src/custom_page.dart';
import 'package:family_chat/src/pages/home/account/account_view.dart';
import 'package:family_chat/src/pages/home/settings/settings_view.dart';
import 'package:flutter/material.dart';

import 'messages/messages_view.dart';
import '../theme_settings/theme_settings_view.dart';

class HomeView extends StatefulWidget {
  const HomeView({Key? key, required this.pageIndex}) : super(key: key);
  final int pageIndex;

  @override
  State<StatefulWidget> createState() => _HomeViewState();
}

class _HomeViewState extends State<HomeView> {
  late int _currentPageIndex;

  goToPage(HomePageEnum page) {
    setState(() {
      _currentPageIndex = page.index;
    });
  }

  late Map<int, CustomPage> pages;

  @override
  void initState() {
    super.initState();

    pages = {
      HomePageEnum.messages.index: MessagesView(goToPage: goToPage),
      HomePageEnum.account.index: AccountView(goToPage: goToPage),
      HomePageEnum.settings.index: SettingView(goToPage: goToPage),
    };

    setState(() {
      _currentPageIndex = widget.pageIndex;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(pages[_currentPageIndex]!.pageName),
        actions: [
          IconButton(
            icon: const Icon(Icons.opacity),
            onPressed: () {
              // Navigate to the settings page. If the user leaves and returns
              // to the app after it has been killed while running in the
              // background, the navigation stack is restored.
              Navigator.pushNamed(context, ThemeSettingsView.routeName);
            },
          ),
        ],
      ),
      body: pages[_currentPageIndex],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentPageIndex,
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.message), label: 'Messages'),
          BottomNavigationBarItem(icon: Icon(Icons.person), label: 'Account'),
          BottomNavigationBarItem(icon: Icon(Icons.settings), label: 'Settings'),
        ],
        onTap: (value) {
          setState(() {
            _currentPageIndex = value;
          });
        },
      ),
      // floatingActionButton: FloatingActionButton(
      //   onPressed: () => {
      //     Navigator.restorablePushNamed(
      //       context,
      //       ChatView.routeName,
      //     )
      //   },
      //   tooltip: 'Open chat',
      //   child: const Icon(Icons.chat),
      // ),
    );
  }
}

enum HomePageEnum { messages, account, settings }
