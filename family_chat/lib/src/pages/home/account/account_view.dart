import 'package:family_chat/src/pages/home/home_view.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../custom_page.dart';
import 'account_controller.dart';
import 'models/user_model.dart';

class AccountView extends StatefulWidget implements CustomPage {
  const AccountView({Key? key, required this.goToPage}) : super(key: key);

  final Function goToPage;

  @override
  String get pageName => "Account";

  @override
  State<StatefulWidget> createState() => _AccountViewState();
}

class _AccountViewState extends State<AccountView> {
  bool loadInterface = false;
  int _userId = 0;
  String _userName = "";
  String _password = "";
  String _avatar = 'https://www.w3schools.com/howto/img_avatar.png';

  bool _loginForm = false;
  bool _showPassword = false;

  var accountController = AccountController();
  var nameController = TextEditingController();
  var passwordController = TextEditingController();
  var avatarController = TextEditingController();

  late SharedPreferences prefs;

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
    _userId = prefs.getInt('userId') ?? 0;

    if (_userId != 0) {
      _setUserData();
      setState(() {
        _loginForm = true;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!loadInterface) return const Center(child: CircularProgressIndicator());

    nameController.text = _userName;
    passwordController.text = _password;
    avatarController.text = _avatar;

    return SingleChildScrollView(
        child: Padding(
      padding: const EdgeInsets.all(15.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          ToggleButtons(
            isSelected: [_loginForm, !_loginForm],
            borderRadius: const BorderRadius.all(Radius.circular(8)),
            selectedBorderColor: Colors.red[700],
            selectedColor: Colors.white,
            fillColor: Colors.red[200],
            color: Colors.red[400],
            constraints: const BoxConstraints(
              minHeight: 40.0,
              minWidth: 80.0,
            ),
            onPressed: (int index) {
              setState(() {
                _loginForm = index == 0 ? true : false;
              });
            },
            children: [
              const Text('Login'),
              Text(_userId == 0 ? 'Register' : 'Update'),
            ],
          ),
          const Divider(height: 30),
          ClipRRect(
            borderRadius: BorderRadius.circular(30),
            child: SizedBox(
              width: 60,
              height: 60,
              child: Image.network(_avatar, fit: BoxFit.cover, errorBuilder: (context, error, stackTrace) {
                return Image.asset("assets/images/wrong-image.png", fit: BoxFit.cover);
              }),
            ),
          ),
          Text("Account (Id: $_userId)"),
          const Divider(),
          Row(
            children: [
              const SizedBox(
                width: 80,
                child: Padding(
                  padding: EdgeInsets.only(right: 10),
                  child: Text("Name: "),
                ),
              ),
              Expanded(
                child: TextField(
                  controller: nameController,
                ),
              ),
            ],
          ),
          Row(
            children: [
              const SizedBox(
                width: 80,
                child: Padding(
                  padding: EdgeInsets.only(right: 10),
                  child: Text("Password: "),
                ),
              ),
              Expanded(
                child: TextField(
                    obscureText: !_showPassword,
                    controller: passwordController,
                    decoration: InputDecoration(
                      suffixIcon: IconButton(
                        icon: Icon(_showPassword ? Icons.visibility_off : Icons.visibility),
                        onPressed: () {
                          setState(() {
                            _showPassword = !_showPassword;
                          });
                        },
                      ),
                    )),
              ),
            ],
          ),
          const Divider(),
          Visibility(
            visible: !_loginForm,
            child: Row(
              children: [
                const SizedBox(
                  width: 80,
                  child: Padding(
                    padding: EdgeInsets.only(right: 10),
                    child: Text("Avatar: "),
                  ),
                ),
                Expanded(
                  child: Focus(
                    child: TextField(
                      controller: avatarController,
                    ),
                    onFocusChange: (hasFocus) {
                      if (!hasFocus) {
                        setState(() {
                          _avatar = avatarController.text;
                        });
                      }
                    },
                  ),
                ),
              ],
            ),
          ),
          const Divider(),
          ElevatedButton.icon(
              label: const Text("Save"),
              onPressed: () async {
                if (nameController.text.isEmpty || avatarController.text.isEmpty) return;

                if (!_loginForm) {
                  // Register or change
                  var userId = await accountController.saveUser(UserModel(
                    id: _userId,
                    userName: nameController.text,
                    password: passwordController.text,
                    avatar: avatarController.text,
                  ));

                  prefs.setInt("userId", userId);

                  setState(() {
                    _userId = userId;
                    _userName = nameController.text;
                    _password = passwordController.text;
                    _avatar = avatarController.text;
                  });
                } else {
                  // Login

                  var user = await accountController.login(
                    nameController.text,
                    passwordController.text,
                  );

                  prefs.setInt("userId", user.id!);

                  setState(() {
                    _userId = user.id!;
                    _userName = user.userName;
                    _password = user.password;
                    _avatar = user.avatar;
                  });
                }

                widget.goToPage(HomePageEnum.messages);
              },
              icon: const Icon(Icons.save))
        ],
      ),
    ));
  }

  _setUserData() async {
    if (_userId == 0) return;
    var userData = await accountController.getUser(_userId);

    setState(() {
      _userName = userData.userName;
      _password = userData.password;
      _avatar = userData.avatar;
    });
  }

  @override
  void dispose() {
    accountController.dispose();
    nameController.dispose();
    avatarController.dispose();
    super.dispose();
  }
}
