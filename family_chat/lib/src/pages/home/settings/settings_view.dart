import 'package:family_chat/src/pages/home/home_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../custom_page.dart';
import 'models/settings_constants.dart';

class SettingView extends StatefulWidget implements CustomPage {
  const SettingView({Key? key, required this.goToPage}) : super(key: key);

  final Function goToPage;

  @override
  String get pageName => "Settings";

  @override
  State<SettingView> createState() {
    return _SettingViewState();
  }
}

class _SettingViewState extends State<SettingView> {
  bool loadInterface = false;
  bool autoValidate = true;
  final _formKey = GlobalKey<FormBuilderState>();
  bool _hostHasError = false;
  bool _portHasError = false;

  // Settings
  late String _protocol;
  late String _host;
  late int _port;

  @override
  void initState() {
    super.initState();

    _readSettingsValues().then((value) {
      setState(() {
        loadInterface = true;
      });
    });
  }

  _readSettingsValues() async {
    final prefs = await SharedPreferences.getInstance();

    _protocol = prefs.getString(SettingsConstants.protocol) ?? 'https';
    _host = prefs.getString(SettingsConstants.host) ?? 'localhost';
    _port = prefs.getInt(SettingsConstants.port) ?? 80;

    // _formKey.currentState?.fields['protocol']?.setValue(protocol);
    // _formKey.currentState?.fields['host']?.setValue(host);
    // _formKey.currentState?.fields['port']?.setValue(port);

    await Future.delayed(const Duration(milliseconds: 500));
  }

  void _onChanged(dynamic val) => debugPrint(val.toString());

  @override
  Widget build(BuildContext context) {
    if (!loadInterface) return const Center(child: CircularProgressIndicator());

    return Padding(
      padding: const EdgeInsets.all(10),
      child: SingleChildScrollView(
        child: Column(
          children: <Widget>[
            FormBuilder(
              key: _formKey,
              // enabled: false,
              onChanged: () {
                _formKey.currentState!.save();
                debugPrint(_formKey.currentState!.value.toString());
              },
              autovalidateMode: AutovalidateMode.disabled,
              initialValue: {
                'protocol': _protocol,
                'host': _host,
                'port': _port.toString(),
              },
              skipDisabled: true,
              child: Column(
                children: <Widget>[
                  const SizedBox(height: 15),
                  FormBuilderSegmentedControl(
                    decoration: const InputDecoration(
                      labelText: 'Protocol',
                    ),
                    name: 'protocol',
                    // initialValue: 1,
                    // textStyle: TextStyle(fontWeight: FontWeight.bold),
                    options: const [
                      FormBuilderFieldOption(
                        value: "https",
                        child: Text(
                          "HTTTPS",
                          style: TextStyle(fontWeight: FontWeight.bold),
                        ),
                      ),
                      FormBuilderFieldOption(
                        value: "http",
                        child: Text(
                          "HTTTP",
                          style: TextStyle(fontWeight: FontWeight.bold),
                        ),
                      ),
                    ],
                    onChanged: _onChanged,
                  ),
                  FormBuilderTextField(
                    autovalidateMode: AutovalidateMode.always,
                    name: 'host',
                    decoration: InputDecoration(
                      labelText: 'Host',
                      suffixIcon: _hostHasError
                          ? const Icon(Icons.error, color: Colors.red)
                          : const Icon(Icons.check, color: Colors.green),
                    ),
                    onChanged: (val) {
                      setState(() {
                        _hostHasError = !(_formKey.currentState?.fields['host']?.validate() ?? false);
                      });
                    },
                    // valueTransformer: (text) => num.tryParse(text),
                    validator: FormBuilderValidators.compose([
                      FormBuilderValidators.required(),
                      FormBuilderValidators.max(200),
                    ]),
                    // initialValue: '12',
                    keyboardType: TextInputType.text,
                    textInputAction: TextInputAction.next,
                  ),
                  FormBuilderTextField(
                    autovalidateMode: AutovalidateMode.always,
                    name: 'port',
                    decoration: InputDecoration(
                      labelText: 'Port',
                      suffixIcon: _portHasError
                          ? const Icon(Icons.error, color: Colors.red)
                          : const Icon(Icons.check, color: Colors.green),
                    ),
                    onChanged: (val) {
                      setState(() {
                        _portHasError = !(_formKey.currentState?.fields['port']?.validate() ?? false);
                      });
                    },
                    // valueTransformer: (text) => num.tryParse(text),
                    validator: FormBuilderValidators.compose([
                      FormBuilderValidators.required(),
                      FormBuilderValidators.numeric(),
                      FormBuilderValidators.max(65536),
                    ]),
                    // initialValue: '12',
                    keyboardType: TextInputType.number,
                    textInputAction: TextInputAction.next,
                  ),
                ],
              ),
            ),
            const Divider(
              height: 30,
            ),
            Row(
              children: <Widget>[
                Expanded(
                  child: ElevatedButton(
                    onPressed: () async {
                      if (!(_formKey.currentState?.saveAndValidate() ?? false)) {
                        debugPrint(_formKey.currentState?.value.toString());
                        debugPrint('validation failed');
                        return;
                      }
                      SharedPreferences prefs = await SharedPreferences.getInstance();

                      prefs.setString(SettingsConstants.protocol, _formKey.currentState?.value['protocol']);
                      prefs.setString(SettingsConstants.host, _formKey.currentState?.value['host']);
                      prefs.setInt(SettingsConstants.port, int.parse(_formKey.currentState?.value['port']));

                      widget.goToPage(HomePageEnum.messages);
                    },
                    child: const Text(
                      'Submit',
                      style: TextStyle(color: Colors.white),
                    ),
                  ),
                ),
                const SizedBox(width: 20),
                Expanded(
                  child: OutlinedButton(
                    onPressed: () {
                      _formKey.currentState?.reset();
                      widget.goToPage(HomePageEnum.messages);
                    },
                    // color: Theme.of(context).colorScheme.secondary,
                    child: Text(
                      'Cancel',
                      style: TextStyle(color: Theme.of(context).colorScheme.secondary),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
